using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Challenges;

public sealed record ChallengeOverview(Challenge Challenge, ChallengeStatus Status, int QuestionCount, int Participants);

public sealed record QuestionScore(int QuestionId, int Order, int Answers, int Correct)
{
    public int Percent => Answers == 0 ? 0 : (int)Math.Round(100.0 * Correct / Answers);
}

public sealed record Scoreboard(int Participants, IReadOnlyList<QuestionScore> Questions);

public enum SubmitOutcome
{
    Saved,
    NotFound,
    NotActive,
    AlreadySubmitted,
    Incomplete,
}

public sealed record SubmitResult(SubmitOutcome Outcome, ChallengeSubmission? Submission = null);

public sealed class ChallengeDraft
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public List<int> QuestionIds { get; set; } = [];
}

public sealed class ChallengeService(ApplicationDbContext db, Reviews.ReviewQueue reviews, LocalCalendar calendar, TimeProvider clock, ILogger<ChallengeService> logger)
{
    public const int TitleMaxLength = 150;
    public const int DescriptionMaxLength = 2000;
    public const int MaxQuestions = 30;

    public DateOnly Today() => calendar.Today();

    /// <summary>Desafios para a página pública; os que ainda não começaram só aparecem para quem pode editar.</summary>
    public async Task<IReadOnlyList<ChallengeOverview>> ListAsync(bool includeUpcoming, CancellationToken cancellationToken = default)
    {
        var today = Today();
        var challenges = await db.Challenges.AsNoTracking()
            .OrderByDescending(c => c.StartsOn).ThenByDescending(c => c.Id)
            .Select(c => new { Challenge = c, QuestionCount = c.Questions.Count, Participants = db.ChallengeSubmissions.Count(s => s.ChallengeId == c.Id) })
            .ToListAsync(cancellationToken);

        return challenges
            .Select(c => new ChallengeOverview(c.Challenge, c.Challenge.StatusOn(today), c.QuestionCount, c.Participants))
            .Where(c => includeUpcoming || c.Status != ChallengeStatus.Upcoming)
            .ToList();
    }

    public Task<Challenge?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        db.Challenges.AsNoTracking()
            .Include(c => c.Questions.OrderBy(q => q.Order))
                .ThenInclude(cq => cq.Question!)
                    .ThenInclude(q => q.Options.OrderBy(o => o.Letter))
            .Include(c => c.Questions)
                .ThenInclude(cq => cq.Question!)
                    .ThenInclude(q => q.Subject)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<ChallengeSubmission?> SubmissionAsync(string userId, int challengeId, CancellationToken cancellationToken = default) =>
        db.ChallengeSubmissions.AsNoTracking().FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.UserId == userId, cancellationToken);

    /// <summary>Alternativas que o aluno marcou neste desafio, por questão.</summary>
    public async Task<IReadOnlyDictionary<int, int>> ChosenOptionsAsync(string userId, int challengeId, CancellationToken cancellationToken = default) =>
        await db.QuestionAttempts.AsNoTracking()
            .Where(a => a.UserId == userId && a.ChallengeId == challengeId)
            .ToDictionaryAsync(a => a.QuestionId, a => a.OptionId, cancellationToken);

    /// <summary>Registra as respostas do aluno. Cada aluno envia uma vez, só enquanto o desafio está aberto e respondendo todas.</summary>
    public async Task<SubmitResult> SubmitAsync(string userId, int challengeId, IReadOnlyDictionary<int, char> answers, CancellationToken cancellationToken = default)
    {
        var challenge = await db.Challenges.AsNoTracking().Include(c => c.Questions).FirstOrDefaultAsync(c => c.Id == challengeId, cancellationToken);
        if (challenge is null)
        {
            return new SubmitResult(SubmitOutcome.NotFound);
        }

        if (challenge.StatusOn(Today()) != ChallengeStatus.Active)
        {
            return new SubmitResult(SubmitOutcome.NotActive);
        }

        if (await db.ChallengeSubmissions.AnyAsync(s => s.ChallengeId == challengeId && s.UserId == userId, cancellationToken))
        {
            return new SubmitResult(SubmitOutcome.AlreadySubmitted);
        }

        var questionIds = challenge.Questions.Select(q => q.QuestionId).ToList();
        var options = await db.QuestionOptions.AsNoTracking().Where(o => questionIds.Contains(o.QuestionId)).ToListAsync(cancellationToken);

        var chosen = new List<QuestionOption>();
        foreach (var questionId in questionIds)
        {
            var option = answers.TryGetValue(questionId, out var letter)
                ? options.FirstOrDefault(o => o.QuestionId == questionId && o.Letter == char.ToUpperInvariant(letter))
                : null;
            if (option is null)
            {
                return new SubmitResult(SubmitOutcome.Incomplete);
            }

            chosen.Add(option);
        }

        var now = clock.GetUtcNow();
        var submission = new ChallengeSubmission
        {
            ChallengeId = challengeId,
            UserId = userId,
            CorrectCount = chosen.Count(o => o.IsCorrect),
            QuestionCount = chosen.Count,
            SubmittedAt = now,
        };

        db.ChallengeSubmissions.Add(submission);
        db.QuestionAttempts.AddRange(chosen.Select(option => new QuestionAttempt
        {
            QuestionId = option.QuestionId,
            UserId = userId,
            OptionId = option.Id,
            IsCorrect = option.IsCorrect,
            AnsweredAt = now,
            ChallengeId = challengeId,
        }));

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Dois envios ao mesmo tempo: a chave (desafio, aluno) deixa passar só o primeiro.
            return new SubmitResult(SubmitOutcome.AlreadySubmitted);
        }

        return new SubmitResult(SubmitOutcome.Saved, submission);
    }

    /// <summary>Placar agregado, sem nomes: quantos participaram e a taxa de acerto de cada questão.</summary>
    public async Task<Scoreboard> ScoreboardAsync(int challengeId, CancellationToken cancellationToken = default)
    {
        var participants = await db.ChallengeSubmissions.CountAsync(s => s.ChallengeId == challengeId, cancellationToken);
        var order = await db.ChallengeQuestions.AsNoTracking()
            .Where(cq => cq.ChallengeId == challengeId)
            .ToDictionaryAsync(cq => cq.QuestionId, cq => cq.Order, cancellationToken);

        var attempts = await db.QuestionAttempts.AsNoTracking()
            .Where(a => a.ChallengeId == challengeId)
            .GroupBy(a => a.QuestionId)
            .Select(g => new { QuestionId = g.Key, Answers = g.Count(), Correct = g.Count(a => a.IsCorrect) })
            .ToListAsync(cancellationToken);

        var scores = order
            .Select(pair =>
            {
                var stats = attempts.FirstOrDefault(a => a.QuestionId == pair.Key);
                return new QuestionScore(pair.Key, pair.Value, stats?.Answers ?? 0, stats?.Correct ?? 0);
            })
            .OrderBy(s => s.Order)
            .ToList();

        return new Scoreboard(participants, scores);
    }

    /// <summary>Questões que podem entrar num desafio: objetivas publicadas.</summary>
    public async Task<IReadOnlyList<Question>> AvailableQuestionsAsync(CancellationToken cancellationToken = default) =>
        await db.Questions.AsNoTracking()
            .Include(q => q.Subject)
            .Where(q => q.Kind == QuestionKind.Objective && q.ArchivedAt == null)
            .OrderBy(q => q.Subject!.Order).ThenBy(q => q.Id)
            .ToListAsync(cancellationToken);

    public async Task<EditResult> CreateAsync(Editor editor, ChallengeDraft draft, CancellationToken cancellationToken = default)
    {
        if (!editor.CanCreate)
        {
            return EditResult.Fail("Só professores e administradores publicam desafios.");
        }

        var errors = await ValidateAsync(draft, cancellationToken);
        if (errors.Count > 0)
        {
            return new EditResult(null, errors);
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, Reviews.ReviewKind.ChallengeCreate, null, draft, draft.Title, cancellationToken);
        }

        var challenge = new Challenge
        {
            TrackId = db.CurrentTrackId,
            Title = draft.Title.Trim(),
            Description = draft.Description.Trim(),
            StartsOn = draft.StartsOn,
            EndsOn = draft.EndsOn,
            AuthorId = editor.UserId,
            CreatedAt = clock.GetUtcNow(),
            Questions = draft.QuestionIds.Select((id, index) => new ChallengeQuestion { QuestionId = id, Order = index + 1 }).ToList(),
        };

        db.Challenges.Add(challenge);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Desafio {ChallengeId} criado por {UserId}.", challenge.Id, editor.UserId);
        return new EditResult(challenge.Id, []);
    }

    public async Task<EditResult> UpdateAsync(Editor editor, int challengeId, ChallengeDraft draft, CancellationToken cancellationToken = default)
    {
        var challenge = await db.Challenges.Include(c => c.Questions).FirstOrDefaultAsync(c => c.Id == challengeId, cancellationToken);
        if (challenge is null)
        {
            return EditResult.Fail("Desafio não encontrado.");
        }

        if (!CanEdit(editor, challenge))
        {
            return EditResult.Fail("Só o autor do desafio ou um administrador pode editá-lo.");
        }

        var errors = await ValidateAsync(draft, cancellationToken);
        var currentIds = challenge.Questions.OrderBy(q => q.Order).Select(q => q.QuestionId).ToList();
        var questionsChanged = !currentIds.SequenceEqual(draft.QuestionIds);
        if (questionsChanged && await db.ChallengeSubmissions.AnyAsync(s => s.ChallengeId == challengeId, cancellationToken))
        {
            // Mudar as questões depois de alguém enviar deixaria o placar comparando provas diferentes.
            errors.Add("As questões não podem mudar depois que algum aluno enviou respostas.");
        }

        if (errors.Count > 0)
        {
            return new EditResult(challengeId, errors);
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, Reviews.ReviewKind.ChallengeUpdate, challengeId, draft, draft.Title, cancellationToken);
        }

        challenge.Title = draft.Title.Trim();
        challenge.Description = draft.Description.Trim();
        challenge.StartsOn = draft.StartsOn;
        challenge.EndsOn = draft.EndsOn;
        if (questionsChanged)
        {
            db.ChallengeQuestions.RemoveRange(challenge.Questions);
            challenge.Questions = draft.QuestionIds.Select((id, index) => new ChallengeQuestion { QuestionId = id, Order = index + 1 }).ToList();
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Desafio {ChallengeId} editado por {UserId}.", challenge.Id, editor.UserId);
        return new EditResult(challenge.Id, []);
    }

    /// <summary>Encerra agora: o gabarito e o placar ficam visíveis.</summary>
    public async Task<EditResult> CloseAsync(Editor editor, int challengeId, CancellationToken cancellationToken = default)
    {
        var challenge = await db.Challenges.FirstOrDefaultAsync(c => c.Id == challengeId, cancellationToken);
        if (challenge is null)
        {
            return EditResult.Fail("Desafio não encontrado.");
        }

        if (!CanEdit(editor, challenge))
        {
            return EditResult.Fail("Só o autor do desafio ou um administrador pode encerrá-lo.");
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, Reviews.ReviewKind.ChallengeClose, challengeId, null, challenge.Title, cancellationToken);
        }

        challenge.ClosedAt ??= clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Desafio {ChallengeId} encerrado por {UserId}.", challenge.Id, editor.UserId);
        return new EditResult(challenge.Id, []);
    }

    public static bool CanEdit(Editor editor, Challenge challenge) =>
        editor.IsAdmin || (editor.IsTeacher && challenge.AuthorId == editor.UserId);

    private async Task<List<string>> ValidateAsync(ChallengeDraft draft, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(draft.Title))
        {
            errors.Add("Preencha o título.");
        }
        else if (draft.Title.Trim().Length > TitleMaxLength)
        {
            errors.Add($"Encurte o título: o limite é {TitleMaxLength} caracteres.");
        }

        if (draft.Description.Trim().Length > DescriptionMaxLength)
        {
            errors.Add($"Encurte a descrição: o limite é {DescriptionMaxLength} caracteres.");
        }

        if (draft.StartsOn == default || draft.EndsOn == default)
        {
            errors.Add("Informe o primeiro e o último dia do desafio.");
        }
        else if (draft.EndsOn < draft.StartsOn)
        {
            errors.Add("O último dia não pode ser antes do primeiro.");
        }

        var ids = draft.QuestionIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            errors.Add("Escolha pelo menos uma questão.");
        }
        else if (ids.Count > MaxQuestions)
        {
            errors.Add($"Escolha no máximo {MaxQuestions} questões.");
        }
        else if (ids.Count != draft.QuestionIds.Count)
        {
            errors.Add("Cada questão só pode entrar uma vez.");
        }
        else
        {
            var valid = await db.Questions.CountAsync(
                q => ids.Contains(q.Id) && q.Kind == QuestionKind.Objective && q.ArchivedAt == null, cancellationToken);
            if (valid != ids.Count)
            {
                errors.Add("Use só questões objetivas publicadas.");
            }
        }

        return errors;
    }
}
