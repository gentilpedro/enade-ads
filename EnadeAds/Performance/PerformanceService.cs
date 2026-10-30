using EnadeAds.Challenges;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Performance;

/// <summary>Acertos sobre questões respondidas, contando só a última resposta de cada questão.</summary>
public sealed record Score(int Answered, int Correct)
{
    public int Percent => Answered == 0 ? 0 : (int)Math.Round(100.0 * Correct / Answered);
}

public sealed record SubjectScore(int SubjectId, string Slug, string Title, StudyBlock Block, int TotalQuestions, Score Score);

public sealed record MistakeItem(int QuestionId, string Excerpt, string SubjectTitle, string SubjectSlug, DateTimeOffset AnsweredAt);

public sealed record ChallengeResult(int ChallengeId, string Title, DateOnly EndsOn, ChallengeStatus Status, int Correct, int Total);

public sealed record PerformanceReport(
    Score Overall,
    IReadOnlyList<SubjectScore> Subjects,
    IReadOnlyList<(StudyBlock Block, Score Score)> Blocks,
    IReadOnlyList<SubjectScore> Weakest,
    IReadOnlyList<MistakeItem> Mistakes,
    int DiscursivesAnswered,
    RoadmapCounts Roadmap,
    IReadOnlyList<ChallengeResult> Challenges);

public sealed class PerformanceService(ApplicationDbContext db, RoadmapService roadmap, LocalCalendar calendar)
{
    private const int ExcerptLength = 140;
    private const int WeakestCount = 3;

    public async Task<PerformanceReport> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var today = calendar.Today();

        // Desafio aberto ainda esconde o gabarito: as respostas dele só entram no painel depois que encerra.
        var challenges = await db.Challenges.AsNoTracking().ToListAsync(cancellationToken);
        var endedChallenges = challenges.Where(c => c.StatusOn(today) == ChallengeStatus.Ended).Select(c => c.Id).ToHashSet();

        var attempts = await db.QuestionAttempts.AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => new { a.Id, a.QuestionId, a.IsCorrect, a.AnsweredAt, a.ChallengeId })
            .ToListAsync(cancellationToken);

        var latest = attempts
            .Where(a => a.ChallengeId is null || endedChallenges.Contains(a.ChallengeId.Value))
            .GroupBy(a => a.QuestionId)
            .Select(g => g.MaxBy(a => a.Id)!)
            .ToList();

        var questions = await db.Questions.AsNoTracking()
            .Include(q => q.Subject)
            .Where(q => q.Kind == QuestionKind.Objective)
            .Select(q => new { q.Id, q.SubjectId, q.Statement, q.ArchivedAt, Subject = q.Subject! })
            .ToListAsync(cancellationToken);
        var questionById = questions.ToDictionary(q => q.Id);

        var subjects = await db.RoadmapNodes.AsNoTracking()
            .Where(n => n.Kind == NodeKind.Subject)
            .OrderBy(n => n.Order)
            .ToListAsync(cancellationToken);

        var subjectScores = subjects.Select(subject =>
        {
            var answered = latest.Where(a => questionById.TryGetValue(a.QuestionId, out var q) && q.SubjectId == subject.Id).ToList();
            var published = questions.Count(q => q.SubjectId == subject.Id && q.ArchivedAt is null);
            return new SubjectScore(subject.Id, subject.Slug, subject.Title, subject.Block, published,
                new Score(answered.Count, answered.Count(a => a.IsCorrect)));
        }).ToList();

        var blocks = subjectScores
            .GroupBy(s => s.Block)
            .OrderBy(g => g.Key)
            .Select(g => (g.Key, new Score(g.Sum(s => s.Score.Answered), g.Sum(s => s.Score.Correct))))
            .ToList();

        // Mais fracas: menor taxa entre as matérias em que o aluno já respondeu algo.
        var weakest = subjectScores
            .Where(s => s.Score.Answered > 0 && s.Score.Correct < s.Score.Answered)
            .OrderBy(s => s.Score.Percent).ThenByDescending(s => s.Score.Answered)
            .Take(WeakestCount)
            .ToList();

        // Caderno de erros: questões cuja última resposta foi errada e que ainda dá para refazer.
        var mistakes = latest
            .Where(a => !a.IsCorrect && questionById.TryGetValue(a.QuestionId, out var q) && q.ArchivedAt is null)
            .Select(a =>
            {
                var question = questionById[a.QuestionId];
                return new MistakeItem(a.QuestionId, Excerpt(question.Statement), question.Subject.Title, question.Subject.Slug, a.AnsweredAt);
            })
            .OrderBy(m => subjects.FindIndex(s => s.Slug == m.SubjectSlug)).ThenBy(m => m.QuestionId)
            .ToList();

        var discursives = await db.DiscursiveAnswers.AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => a.QuestionId)
            .Distinct()
            .CountAsync(cancellationToken);

        var submissions = await db.ChallengeSubmissions.AsNoTracking()
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
        var challengeResults = submissions
            .Join(challenges, s => s.ChallengeId, c => c.Id,
                (s, c) => new ChallengeResult(c.Id, c.Title, c.EndsOn, c.StatusOn(today), s.CorrectCount, s.QuestionCount))
            .OrderByDescending(r => r.EndsOn)
            .ToList();

        var roadmapCounts = (await roadmap.GetAsync(userId, cancellationToken)).Counts;
        var overall = new Score(latest.Count(a => questionById.ContainsKey(a.QuestionId)), latest.Count(a => a.IsCorrect && questionById.ContainsKey(a.QuestionId)));

        return new PerformanceReport(overall, subjectScores, blocks, weakest, mistakes, discursives, roadmapCounts, challengeResults);
    }

    private static string Excerpt(string statement)
    {
        var flat = string.Join(' ', statement.Split((char[])['\n', '\r', '\t', ' '], StringSplitOptions.RemoveEmptyEntries));
        return flat.Length <= ExcerptLength ? flat : $"{flat[..ExcerptLength].TrimEnd()}…";
    }
}
