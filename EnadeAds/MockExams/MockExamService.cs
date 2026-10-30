using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.MockExams;

public sealed record MockExamPlan(MockExamFormat Format, int GeneralEducation, int Specific, int Discursive)
{
    public int Total => GeneralEducation + Specific + Discursive;
    public int Minutes => MockExamFormat.MinutesFor(Total);
    public bool IsShorterThanFormat => Total < Format.Total;
}

public enum StartOutcome
{
    Started,
    AlreadyOpen,
    NoQuestions,
}

public sealed record StartResult(StartOutcome Outcome, int? MockExamId);

public enum SaveOutcome
{
    Saved,
    Submitted,
    NotFound,
    AlreadyClosed,
}

public sealed class MockExamService(ApplicationDbContext db, TimeProvider clock, ILogger<MockExamService> logger)
{
    public const string GeneralEducationSlug = "formacao-geral";
    public const int DiscursiveMaxLength = QuestionBank.DiscursiveAnswerMaxLength;

    /// <summary>Tolerância para a resposta enviada no último segundo ainda chegar dentro do prazo.</summary>
    public static readonly TimeSpan GracePeriod = TimeSpan.FromMinutes(1);

    /// <summary>Quantas questões cada tamanho terá com o banco atual (o banco pode ter menos do que o formato pede).</summary>
    public async Task<IReadOnlyList<MockExamPlan>> PlansAsync(CancellationToken cancellationToken = default)
    {
        var pools = await PoolsAsync(cancellationToken);
        return MockExamFormat.All.Select(format => Plan(format, pools)).ToList();
    }

    /// <summary>Simulado em andamento do aluno, já entregando por tempo se o prazo passou.</summary>
    public async Task<MockExam?> OpenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var open = await db.MockExams.FirstOrDefaultAsync(m => m.UserId == userId && m.SubmittedAt == null, cancellationToken);
        if (open is null)
        {
            return null;
        }

        await CloseIfExpiredAsync(open.Id, cancellationToken);
        return await db.MockExams.AsNoTracking().FirstOrDefaultAsync(m => m.Id == open.Id && m.SubmittedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<MockExam>> HistoryAsync(string userId, CancellationToken cancellationToken = default) =>
        await db.MockExams.AsNoTracking()
            .Where(m => m.UserId == userId && m.SubmittedAt != null)
            .OrderByDescending(m => m.Id)
            .ToListAsync(cancellationToken);

    public async Task<StartResult> StartAsync(string userId, MockExamSize size, CancellationToken cancellationToken = default)
    {
        if (await OpenAsync(userId, cancellationToken) is { } open)
        {
            return new StartResult(StartOutcome.AlreadyOpen, open.Id);
        }

        var pools = await PoolsAsync(cancellationToken);
        var plan = Plan(MockExamFormat.Of(size), pools);
        if (plan.Total == 0)
        {
            return new StartResult(StartOutcome.NoQuestions, null);
        }

        // Ordem da prova: formação geral, específicas e, por último, a discursiva.
        var picked = Pick(pools.GeneralEducation, plan.GeneralEducation).Select(id => (id, general: true))
            .Concat(Pick(pools.Specific, plan.Specific).Select(id => (id, general: false)))
            .Concat(Pick(pools.Discursive, plan.Discursive).Select(id => (id, general: false)))
            .ToList();

        var now = clock.GetUtcNow();
        var exam = new MockExam
        {
            UserId = userId,
            Size = size,
            StartedAt = now,
            DeadlineAt = now.AddMinutes(plan.Minutes),
            ObjectiveCount = plan.GeneralEducation + plan.Specific,
            Questions = picked.Select((item, index) => new MockExamQuestion
            {
                QuestionId = item.id,
                Order = index + 1,
                IsGeneralEducation = item.general,
            }).ToList(),
        };

        db.MockExams.Add(exam);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Simulado {MockExamId} iniciado ({Size}, {Count} questões).", exam.Id, size, picked.Count);
        return new StartResult(StartOutcome.Started, exam.Id);
    }

    /// <summary>Simulado do próprio aluno com as questões; nulo se não existir ou for de outra pessoa.</summary>
    public async Task<MockExam?> FindAsync(string userId, int mockExamId, CancellationToken cancellationToken = default)
    {
        await CloseIfExpiredAsync(mockExamId, cancellationToken);

        return await db.MockExams.AsNoTracking()
            .Where(m => m.Id == mockExamId && m.UserId == userId)
            .Include(m => m.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.Question!)
                    .ThenInclude(q => q.Options.OrderBy(o => o.Letter))
            .Include(m => m.Questions)
                .ThenInclude(q => q.Question!)
                    .ThenInclude(q => q.Subject)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>Salva as respostas; com <paramref name="submit"/>, entrega e corrige.</summary>
    public async Task<SaveOutcome> SaveAsync(string userId, int mockExamId, IReadOnlyDictionary<int, char> letters,
        IReadOnlyDictionary<int, string> texts, bool submit, CancellationToken cancellationToken = default)
    {
        var exam = await db.MockExams.Include(m => m.Questions).ThenInclude(q => q.Question!).ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(m => m.Id == mockExamId && m.UserId == userId, cancellationToken);
        if (exam is null)
        {
            return SaveOutcome.NotFound;
        }

        if (!exam.IsOpen)
        {
            return SaveOutcome.AlreadyClosed;
        }

        var now = clock.GetUtcNow();
        var expired = now > exam.DeadlineAt + GracePeriod;

        // Depois do prazo, o que chega agora é ignorado: vale o que estava salvo até o fim do tempo.
        if (!expired)
        {
            foreach (var item in exam.Questions)
            {
                var question = item.Question!;
                if (question.Kind == QuestionKind.Objective && letters.TryGetValue(item.QuestionId, out var letter))
                {
                    item.SelectedOptionId = question.Options.FirstOrDefault(o => o.Letter == char.ToUpperInvariant(letter))?.Id ?? item.SelectedOptionId;
                }
                else if (question.Kind == QuestionKind.Discursive && texts.TryGetValue(item.QuestionId, out var text))
                {
                    var trimmed = text.Replace("\r\n", "\n").Trim();
                    item.DiscursiveText = trimmed.Length == 0 ? null : trimmed[..Math.Min(trimmed.Length, DiscursiveMaxLength)];
                }
            }
        }

        if (submit || expired)
        {
            Grade(exam, now, byTimeout: expired);
        }

        await db.SaveChangesAsync(cancellationToken);
        return exam.IsOpen ? SaveOutcome.Saved : SaveOutcome.Submitted;
    }

    private async Task CloseIfExpiredAsync(int mockExamId, CancellationToken cancellationToken)
    {
        var exam = await db.MockExams.Include(m => m.Questions).ThenInclude(q => q.Question!).ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(m => m.Id == mockExamId, cancellationToken);
        var now = clock.GetUtcNow();
        if (exam is not { IsOpen: true } || now <= exam.DeadlineAt + GracePeriod)
        {
            return;
        }

        Grade(exam, now, byTimeout: true);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Corrige e grava as respostas como tentativas, para o painel de desempenho e o caderno de erros.</summary>
    private void Grade(MockExam exam, DateTimeOffset now, bool byTimeout)
    {
        var correct = 0;
        foreach (var item in exam.Questions)
        {
            var question = item.Question!;
            if (question.Kind == QuestionKind.Objective && item.SelectedOptionId is { } optionId)
            {
                var option = question.Options.First(o => o.Id == optionId);
                correct += option.IsCorrect ? 1 : 0;
                db.QuestionAttempts.Add(new QuestionAttempt
                {
                    QuestionId = question.Id, UserId = exam.UserId, OptionId = option.Id, IsCorrect = option.IsCorrect, AnsweredAt = now,
                });
            }
            else if (question.Kind == QuestionKind.Discursive && item.DiscursiveText is { } text)
            {
                db.DiscursiveAnswers.Add(new DiscursiveAnswer { QuestionId = question.Id, UserId = exam.UserId, Text = text, AnsweredAt = now });
            }
        }

        exam.CorrectCount = correct;
        exam.SubmittedAt = now;
        exam.SubmittedByTimeout = byTimeout;
        logger.LogInformation("Simulado {MockExamId} entregue ({Reason}): {Correct} de {Total}.",
            exam.Id, byTimeout ? "tempo esgotado" : "aluno", correct, exam.ObjectiveCount);
    }

    private sealed record Pools(List<int> GeneralEducation, List<int> Specific, List<int> Discursive);

    private async Task<Pools> PoolsAsync(CancellationToken cancellationToken)
    {
        var questions = await db.Questions.AsNoTracking()
            .Where(q => q.ArchivedAt == null)
            .Select(q => new { q.Id, q.Kind, Slug = q.Subject!.Slug })
            .ToListAsync(cancellationToken);

        return new Pools(
            questions.Where(q => q.Kind == QuestionKind.Objective && q.Slug == GeneralEducationSlug).Select(q => q.Id).ToList(),
            questions.Where(q => q.Kind == QuestionKind.Objective && q.Slug != GeneralEducationSlug).Select(q => q.Id).ToList(),
            questions.Where(q => q.Kind == QuestionKind.Discursive).Select(q => q.Id).ToList());
    }

    /// <summary>
    /// Respeita o formato quando o banco permite. Se faltar formação geral, completa com específicas e
    /// vice-versa, para o aluno ter o simulado mais completo possível.
    /// </summary>
    private static MockExamPlan Plan(MockExamFormat format, Pools pools)
    {
        var general = Math.Min(format.GeneralEducation, pools.GeneralEducation.Count);
        var specific = Math.Min(format.Specific, pools.Specific.Count);

        var objectiveSlots = format.GeneralEducation + format.Specific;
        var missing = objectiveSlots - general - specific;
        var extraSpecific = Math.Min(missing, pools.Specific.Count - specific);
        specific += extraSpecific;
        general += Math.Min(missing - extraSpecific, pools.GeneralEducation.Count - general);

        return new MockExamPlan(format, general, specific, Math.Min(format.Discursive, pools.Discursive.Count));
    }

    private static IEnumerable<int> Pick(List<int> pool, int count) => pool.OrderBy(_ => Random.Shared.Next()).Take(count);
}
