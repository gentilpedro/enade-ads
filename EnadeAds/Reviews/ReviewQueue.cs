using System.Text.Json;
using EnadeAds.Data;
using EnadeAds.Questions;

namespace EnadeAds.Reviews;

/// <summary>
/// Guarda a alteração de um professor para revisão em vez de aplicar. Os serviços de edição chamam isto quando
/// <see cref="Editor.NeedsReview"/>; quem aplica depois é o <see cref="ReviewDecisions"/>.
/// </summary>
public sealed class ReviewQueue(ApplicationDbContext db, TimeProvider clock, ILogger<ReviewQueue> logger)
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public const string QueuedMessage = "Enviado para revisão. A alteração passa a valer quando outro professor ou um admin aprovar em Revisões.";

    /// <summary>Página de revisões destacando a proposta recém-enviada.</summary>
    public static string SentUrl(EditResult result) => $"revisoes?enviada={result.QueuedReviewId}";

    public async Task<EditResult> SubmitAsync(Editor author, ReviewKind kind, int? targetId, object? payload, string summary,
        CancellationToken cancellationToken = default)
    {
        var review = new ContentReview
        {
            TrackId = db.CurrentTrackId,
            Kind = kind,
            TargetId = targetId,
            Payload = payload is null ? "{}" : JsonSerializer.Serialize(payload, payload.GetType(), Json),
            Summary = Shorten(summary),
            AuthorId = author.UserId,
            CreatedAt = clock.GetUtcNow(),
            Status = ReviewStatus.Pending,
        };

        db.ContentReviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Proposta {ReviewId} ({Kind}) enviada para revisão por {UserId}.", review.Id, kind, author.UserId);
        return EditResult.Queued(review.Id);
    }

    private static string Shorten(string text)
    {
        var flat = string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return flat.Length <= ContentReview.SummaryMaxLength ? flat : flat[..(ContentReview.SummaryMaxLength - 1)] + "…";
    }
}
