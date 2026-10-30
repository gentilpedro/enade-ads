using System.Text.Json;
using EnadeAds.Challenges;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Reviews;

/// <summary>Uma proposta com o que é preciso para revisar: quem enviou, em que trilha e o conteúdo proposto.</summary>
public sealed record ReviewItem(
    ContentReview Review,
    string AuthorName,
    string TrackName,
    string? TargetTitle,
    QuestionDraft? Question,
    ChallengeDraft? Challenge,
    RoadmapTextPayload? Text,
    RoadmapMovePayload? Move);

public sealed record DecisionResult(bool Succeeded, string Message);

/// <summary>
/// Revisão das alterações de professores. Aprova outro professor ou um admin, nunca o próprio autor.
/// Na aprovação a alteração roda de novo pelo serviço de edição, como o autor e na trilha da proposta,
/// então passa pelas mesmas validações; se o item mudou desde a proposta e ela não vale mais, fica como "não aplicada".
/// </summary>
public sealed class ReviewDecisions(ApplicationDbContext db, IServiceScopeFactory scopes, TimeProvider clock, ILogger<ReviewDecisions> logger)
{
    public static bool CanReview(Editor reviewer, ContentReview review) =>
        reviewer.IsAdmin || (reviewer.IsTeacher && review.AuthorId != reviewer.UserId);

    public async Task<int> PendingCountAsync(CancellationToken cancellationToken = default) =>
        await db.ContentReviews.CountAsync(r => r.Status == ReviewStatus.Pending, cancellationToken);

    public async Task<IReadOnlyList<ReviewItem>> PendingAsync(CancellationToken cancellationToken = default) =>
        await ItemsAsync(db.ContentReviews.Where(r => r.Status == ReviewStatus.Pending).OrderBy(r => r.CreatedAt), cancellationToken);

    /// <summary>As últimas propostas do próprio professor, para ele acompanhar o que foi aprovado ou recusado.</summary>
    public async Task<IReadOnlyList<ReviewItem>> MineAsync(string userId, CancellationToken cancellationToken = default) =>
        await ItemsAsync(db.ContentReviews.Where(r => r.AuthorId == userId).OrderByDescending(r => r.CreatedAt).Take(20), cancellationToken);

    public async Task<DecisionResult> ApproveAsync(Editor reviewer, int reviewId, CancellationToken cancellationToken = default)
    {
        var review = await db.ContentReviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
        if (review is null || review.Status != ReviewStatus.Pending)
        {
            return new(false, "Essa proposta não está mais aguardando revisão.");
        }

        if (!CanReview(reviewer, review))
        {
            return new(false, "Quem enviou a proposta não pode aprová-la: precisa ser outro professor ou um admin.");
        }

        EditResult result;
        try
        {
            result = await ApplyAsync(review, cancellationToken);
        }
        catch (JsonException error)
        {
            logger.LogError(error, "Proposta {ReviewId} com conteúdo inválido.", review.Id);
            result = EditResult.Fail("O conteúdo da proposta está corrompido.");
        }

        review.Status = result.Succeeded ? ReviewStatus.Approved : ReviewStatus.Failed;
        review.ResultId = result.Id;
        review.Note = result.Succeeded ? null : Limit(string.Join(" ", result.Errors));
        review.ReviewerId = reviewer.UserId;
        review.ReviewedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Proposta {ReviewId} aprovada por {UserId}: {Status}.", review.Id, reviewer.UserId, review.Status);

        return result.Succeeded
            ? new(true, "Proposta aprovada e publicada.")
            : new(false, $"A proposta foi aprovada, mas não pôde ser aplicada: {review.Note}");
    }

    public async Task<DecisionResult> RejectAsync(Editor reviewer, int reviewId, string? note, CancellationToken cancellationToken = default)
    {
        var review = await db.ContentReviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
        if (review is null || review.Status != ReviewStatus.Pending)
        {
            return new(false, "Essa proposta não está mais aguardando revisão.");
        }

        // O próprio autor pode retirar a proposta; recusar a de outra pessoa é de revisor.
        if (review.AuthorId != reviewer.UserId && !CanReview(reviewer, review))
        {
            return new(false, "Só outro professor ou um admin pode recusar a proposta.");
        }

        review.Status = ReviewStatus.Rejected;
        review.Note = string.IsNullOrWhiteSpace(note) ? null : Limit(note.Trim());
        review.ReviewerId = reviewer.UserId;
        review.ReviewedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Proposta {ReviewId} recusada por {UserId}.", review.Id, reviewer.UserId);
        return new(true, review.AuthorId == reviewer.UserId ? "Proposta retirada." : "Proposta recusada.");
    }

    /// <summary>Roda a alteração pelo serviço de edição, como o autor e na trilha em que ela foi proposta.</summary>
    private async Task<EditResult> ApplyAsync(ContentReview review, CancellationToken cancellationToken)
    {
        using var scope = scopes.CreateScope();
        var services = scope.ServiceProvider;
        services.GetRequiredService<TrackScope>().Use(review.TrackId);
        var author = new Editor(review.AuthorId, IsAdmin: false, IsTeacher: true, Reviewed: true);
        var target = review.TargetId ?? 0;

        var questions = services.GetRequiredService<QuestionEditor>();
        var roadmap = services.GetRequiredService<RoadmapEditor>();
        var challenges = services.GetRequiredService<ChallengeService>();

        return review.Kind switch
        {
            ReviewKind.QuestionCreate => await questions.CreateAsync(author, Read<QuestionDraft>(review), cancellationToken),
            ReviewKind.QuestionUpdate => await questions.UpdateAsync(author, target, Read<QuestionDraft>(review), cancellationToken),
            ReviewKind.QuestionArchive => await questions.ArchiveAsync(author, target, cancellationToken),
            ReviewKind.RoadmapUpdate => await roadmap.UpdateAsync(author, target, Read<RoadmapTextPayload>(review).Title, Read<RoadmapTextPayload>(review).Summary, cancellationToken),
            ReviewKind.RoadmapAddTopic => await roadmap.AddTopicAsync(author, target, Read<RoadmapTextPayload>(review).Title, Read<RoadmapTextPayload>(review).Summary, cancellationToken),
            ReviewKind.RoadmapAddCheckpoint => await roadmap.AddCheckpointAfterAsync(author, target, Read<RoadmapTextPayload>(review).Title, Read<RoadmapTextPayload>(review).Summary, cancellationToken),
            ReviewKind.RoadmapMove => await roadmap.MoveAsync(author, target, Read<RoadmapMovePayload>(review).Up, cancellationToken),
            ReviewKind.RoadmapRemove => await roadmap.RemoveAsync(author, target, cancellationToken),
            ReviewKind.ChallengeCreate => await challenges.CreateAsync(author, Read<ChallengeDraft>(review), cancellationToken),
            ReviewKind.ChallengeUpdate => await challenges.UpdateAsync(author, target, Read<ChallengeDraft>(review), cancellationToken),
            ReviewKind.ChallengeClose => await challenges.CloseAsync(author, target, cancellationToken),
            _ => EditResult.Fail("Tipo de proposta desconhecido."),
        };
    }

    private static T Read<T>(ContentReview review) =>
        JsonSerializer.Deserialize<T>(review.Payload, ReviewQueue.Json) ?? throw new JsonException("Proposta vazia.");

    private async Task<IReadOnlyList<ReviewItem>> ItemsAsync(IQueryable<ContentReview> query, CancellationToken cancellationToken)
    {
        var reviews = await query.AsNoTracking().ToListAsync(cancellationToken);
        var authorIds = reviews.Select(r => r.AuthorId).Distinct().ToList();
        var authors = await db.Users.Where(u => authorIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);
        var tracks = await db.Tracks.AsNoTracking().ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var items = new List<ReviewItem>();
        foreach (var review in reviews)
        {
            var question = review.Kind is ReviewKind.QuestionCreate or ReviewKind.QuestionUpdate ? TryRead<QuestionDraft>(review) : null;
            var challenge = review.Kind is ReviewKind.ChallengeCreate or ReviewKind.ChallengeUpdate ? TryRead<ChallengeDraft>(review) : null;
            var text = review.Kind is ReviewKind.RoadmapUpdate or ReviewKind.RoadmapAddTopic or ReviewKind.RoadmapAddCheckpoint ? TryRead<RoadmapTextPayload>(review) : null;
            var move = review.Kind == ReviewKind.RoadmapMove ? TryRead<RoadmapMovePayload>(review) : null;

            items.Add(new ReviewItem(review, authors.GetValueOrDefault(review.AuthorId, "Conta excluída"),
                tracks.GetValueOrDefault(review.TrackId, ""), await TargetTitleAsync(review, cancellationToken),
                question, challenge, text, move));
        }

        return items;
    }

    /// <summary>Como o item alterado está hoje (de qualquer trilha), para o revisor comparar.</summary>
    private async Task<string?> TargetTitleAsync(ContentReview review, CancellationToken cancellationToken)
    {
        if (review.TargetId is not { } id)
        {
            return null;
        }

        return review.Kind switch
        {
            ReviewKind.QuestionUpdate or ReviewKind.QuestionArchive => await db.Questions.IgnoreQueryFilters()
                .Where(q => q.Id == id).Select(q => q.Statement).FirstOrDefaultAsync(cancellationToken),
            ReviewKind.ChallengeUpdate or ReviewKind.ChallengeClose => await db.Challenges.IgnoreQueryFilters()
                .Where(c => c.Id == id).Select(c => c.Title).FirstOrDefaultAsync(cancellationToken),
            _ => await db.RoadmapNodes.IgnoreQueryFilters().Where(n => n.Id == id).Select(n => n.Title).FirstOrDefaultAsync(cancellationToken),
        };
    }

    private T? TryRead<T>(ContentReview review) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(review.Payload, ReviewQueue.Json);
        }
        catch (JsonException error)
        {
            logger.LogWarning(error, "Proposta {ReviewId} com conteúdo ilegível.", review.Id);
            return null;
        }
    }

    private static string Limit(string text) =>
        text.Length <= ContentReview.NoteMaxLength ? text : text[..(ContentReview.NoteMaxLength - 1)] + "…";
}
