using EnadeAds.Data;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Questions;

public enum AttemptOutcome
{
    NotAnswered,
    Correct,
    Wrong,
}

public sealed record QuestionSummary(int Id, string Excerpt, string SubjectTitle, string SubjectSlug, string Source, AttemptOutcome Outcome);

public sealed record QuestionPage(IReadOnlyList<QuestionSummary> Items, int Page, int TotalPages, int TotalCount);

public sealed record AnswerResult(QuestionAttempt Attempt, QuestionOption Chosen);

public sealed class QuestionBank(ApplicationDbContext db, TimeProvider clock)
{
    public const int PageSize = 20;
    private const int ExcerptLength = 180;

    public async Task<QuestionPage> ListAsync(string? subjectSlug, int page, string? userId, CancellationToken cancellationToken = default)
    {
        var query = db.Questions.AsNoTracking().Include(q => q.Subject).Where(q => q.ArchivedAt == null);
        if (!string.IsNullOrEmpty(subjectSlug))
        {
            query = query.Where(q => q.Subject!.Slug == subjectSlug);
        }

        var total = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var questions = await query
            .OrderBy(q => q.Subject!.Order).ThenBy(q => q.Id)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .ToListAsync(cancellationToken);

        var outcomes = await LatestOutcomesAsync(userId, questions.Select(q => q.Id).ToList(), cancellationToken);

        var items = questions.Select(q => new QuestionSummary(
            q.Id, Excerpt(q.Statement), q.Subject!.Title, q.Subject.Slug, q.Source,
            outcomes.GetValueOrDefault(q.Id, AttemptOutcome.NotAnswered))).ToList();

        return new QuestionPage(items, page, totalPages, total);
    }

    /// <summary>Questão publicada (não arquivada).</summary>
    public async Task<Question?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        var question = await FindIncludingArchivedAsync(id, cancellationToken);
        return question is { ArchivedAt: null } ? question : null;
    }

    /// <summary>Inclui arquivadas: usado só na edição, por quem pode editar.</summary>
    public Task<Question?> FindIncludingArchivedAsync(int id, CancellationToken cancellationToken = default) =>
        db.Questions.AsNoTracking()
            .Include(q => q.Subject)
            .Include(q => q.Options.OrderBy(o => o.Letter))
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

    public Task<int> CountBySubjectAsync(int subjectId, CancellationToken cancellationToken = default) =>
        db.Questions.CountAsync(q => q.SubjectId == subjectId && q.ArchivedAt == null, cancellationToken);

    public Task<QuestionAttempt?> LatestAttemptAsync(string userId, int questionId, CancellationToken cancellationToken = default) =>
        db.QuestionAttempts.AsNoTracking()
            .Where(a => a.UserId == userId && a.QuestionId == questionId)
            // O id cresce a cada resposta: a maior é a mais recente (e funciona em qualquer banco).
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>Registra a resposta do aluno. Retorna nulo se a questão ou a alternativa não existirem.</summary>
    public async Task<AnswerResult?> AnswerAsync(string userId, int questionId, char letter, CancellationToken cancellationToken = default)
    {
        var option = await db.QuestionOptions.AsNoTracking()
            .FirstOrDefaultAsync(o => o.QuestionId == questionId && o.Letter == char.ToUpperInvariant(letter), cancellationToken);
        if (option is null)
        {
            return null;
        }

        var attempt = new QuestionAttempt
        {
            QuestionId = questionId,
            UserId = userId,
            OptionId = option.Id,
            IsCorrect = option.IsCorrect,
            AnsweredAt = clock.GetUtcNow(),
        };
        db.QuestionAttempts.Add(attempt);
        await db.SaveChangesAsync(cancellationToken);

        return new AnswerResult(attempt, option);
    }

    private async Task<Dictionary<int, AttemptOutcome>> LatestOutcomesAsync(string? userId, List<int> questionIds, CancellationToken cancellationToken)
    {
        if (userId is null || questionIds.Count == 0)
        {
            return [];
        }

        var attempts = await db.QuestionAttempts.AsNoTracking()
            .Where(a => a.UserId == userId && questionIds.Contains(a.QuestionId))
            .Select(a => new { a.QuestionId, a.IsCorrect, a.Id })
            .ToListAsync(cancellationToken);

        return attempts
            .GroupBy(a => a.QuestionId)
            .ToDictionary(
                g => g.Key,
                g => g.MaxBy(a => a.Id)!.IsCorrect ? AttemptOutcome.Correct : AttemptOutcome.Wrong);
    }

    private static string Excerpt(string statement)
    {
        var flat = string.Join(' ', statement.Split((char[])['\n', '\r', '\t', ' '], StringSplitOptions.RemoveEmptyEntries));
        return flat.Length <= ExcerptLength ? flat : $"{flat[..ExcerptLength].TrimEnd()}…";
    }
}
