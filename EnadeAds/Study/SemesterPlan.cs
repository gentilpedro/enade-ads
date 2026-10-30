using System.Globalization;
using EnadeAds.Challenges;
using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Study;

/// <summary><see cref="Block"/> só vem preenchido quando todas as questões do desafio são do mesmo bloco do roadmap.</summary>
public sealed record WeekChallenge(int Id, string Title, ChallengeStatus Status, bool Done, StudyBlock? Block);

/// <summary><see cref="ExamDays"/> são os dias de prova que caem na semana (o ENEM tem dois, em semanas diferentes).</summary>
public sealed record WeekSlot(int Number, DateOnly Start, DateOnly End, bool IsCurrent, IReadOnlyList<DateOnly> ExamDays, IReadOnlyList<WeekChallenge> Challenges)
{
    public bool HasExam => ExamDays.Count > 0;
}

/// <summary>
/// As semanas de hoje até o último dia de prova; <see cref="SkippedWeeks"/> conta as do meio que não couberam na faixa.
/// <see cref="ExamDate"/> é o próximo dia de prova, para a contagem regressiva.
/// </summary>
public sealed record SemesterView(Track Track, DateOnly ExamDate, int DaysLeft, IReadOnlyList<WeekSlot> Weeks, int SkippedWeeks);

/// <summary>
/// A faixa do semestre da página inicial: uma coluna por semana (segunda a domingo) até a semana da prova,
/// com os desafios de cada semana e se o aluno já entregou.
/// </summary>
public sealed class SemesterPlan(ApplicationDbContext db, TrackCatalog tracks, LocalCalendar calendar)
{
    /// <summary>Colunas na faixa. Com mais semanas, mostra as primeiras e a da prova, e conta as do meio.</summary>
    public const int MaxColumns = 6;

    public async Task<SemesterView?> BuildAsync(string? userId, bool canSeeUpcoming, CancellationToken cancellationToken = default)
    {
        var today = calendar.Today();
        var track = await tracks.CurrentAsync(cancellationToken);
        if (track.LastExamDay < today)
        {
            return null;
        }

        var examDate = track.ExamDays.Where(day => day >= today).Min();
        var weeks = Weeks(today, track.LastExamDay, MaxColumns, out var skipped);
        var first = weeks[0].Start;
        var last = weeks[^1].End;

        var challenges = await db.Challenges.AsNoTracking()
            .Where(c => c.StartsOn <= last && c.EndsOn >= first)
            .OrderBy(c => c.StartsOn).ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);
        var ids = challenges.Select(c => c.Id).ToList();
        var blocks = (await db.ChallengeQuestions.Where(cq => ids.Contains(cq.ChallengeId))
                .Join(db.Questions, cq => cq.QuestionId, q => q.Id, (cq, q) => new { cq.ChallengeId, q.SubjectId })
                .Join(db.RoadmapNodes, x => x.SubjectId, n => n.Id, (x, n) => new { x.ChallengeId, n.Block })
                .ToListAsync(cancellationToken))
            .GroupBy(x => x.ChallengeId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Block).Distinct().ToList() is [var only] ? only : (StudyBlock?)null);
        var done = userId is null
            ? []
            : (await db.ChallengeSubmissions.Where(s => s.UserId == userId && ids.Contains(s.ChallengeId)).Select(s => s.ChallengeId).ToListAsync(cancellationToken)).ToHashSet();

        var visible = challenges
            .Select(c => new { c.StartsOn, Item = new WeekChallenge(c.Id, c.Title, c.StatusOn(today), done.Contains(c.Id), blocks.GetValueOrDefault(c.Id)) })
            // Desafio agendado só aparece para quem pode editar, como na página de desafios.
            .Where(c => canSeeUpcoming || c.Item.Status != ChallengeStatus.Upcoming)
            .ToList();

        var slots = weeks.Select((week, index) => new WeekSlot(
            week.Number, week.Start, week.End,
            IsCurrent: index == 0,
            ExamDays: track.ExamDays.Where(day => day >= week.Start && day <= week.End && day >= today).ToList(),
            // Cada desafio entra na semana em que começa (ou na primeira coluna, se começou antes).
            visible.Where(c => WeekOf(c.StartsOn < first ? first : c.StartsOn) == week.Start).Select(c => c.Item).ToList())).ToList();

        return new SemesterView(track, examDate, examDate.DayNumber - today.DayNumber, slots, skipped);
    }

    /// <summary>Semanas de segunda a domingo, da atual até a da prova. Se passar de <paramref name="maxColumns"/>, guarda as primeiras e a da prova.</summary>
    public static IReadOnlyList<(int Number, DateOnly Start, DateOnly End)> Weeks(DateOnly today, DateOnly examDate, int maxColumns, out int skipped)
    {
        var all = new List<(int, DateOnly, DateOnly)>();
        for (var start = WeekOf(today); start <= WeekOf(examDate); start = start.AddDays(7))
        {
            all.Add((ISOWeek.GetWeekOfYear(start.ToDateTime(TimeOnly.MinValue)), start, start.AddDays(6)));
        }

        skipped = Math.Max(0, all.Count - maxColumns);
        return skipped == 0 ? all : [.. all.Take(maxColumns - 1), all[^1]];
    }

    private static DateOnly WeekOf(DateOnly date) => date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
}
