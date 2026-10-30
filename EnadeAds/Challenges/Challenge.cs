using EnadeAds.Questions;

namespace EnadeAds.Challenges;

public enum ChallengeStatus
{
    Upcoming,
    Active,
    Ended,
}

/// <summary>Desafio publicado por um professor: um conjunto de questões objetivas para responder num período.</summary>
public class Challenge
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    /// <summary>Primeiro e último dia do desafio, no fuso da plataforma (inclusive).</summary>
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }

    /// <summary>Preenchido quando o professor encerra antes do fim do período.</summary>
    public DateTimeOffset? ClosedAt { get; set; }

    public string? AuthorId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<ChallengeQuestion> Questions { get; set; } = [];

    public ChallengeStatus StatusOn(DateOnly today) =>
        ClosedAt is not null || today > EndsOn ? ChallengeStatus.Ended
        : today < StartsOn ? ChallengeStatus.Upcoming
        : ChallengeStatus.Active;
}

public class ChallengeQuestion
{
    public int ChallengeId { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    public int Order { get; set; }
}

/// <summary>Envio de um aluno num desafio. As respostas ficam em <see cref="QuestionAttempt"/> com o mesmo desafio.</summary>
public class ChallengeSubmission
{
    public int ChallengeId { get; set; }
    public required string UserId { get; set; }
    public int CorrectCount { get; set; }
    public int QuestionCount { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
}
