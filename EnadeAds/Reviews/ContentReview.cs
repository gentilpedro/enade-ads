namespace EnadeAds.Reviews;

/// <summary>O que a proposta faz. Cada tipo corresponde a um método de edição de questões, roadmap ou desafios.</summary>
public enum ReviewKind
{
    QuestionCreate = 0,
    QuestionUpdate = 1,
    QuestionArchive = 2,
    RoadmapUpdate = 10,
    RoadmapAddTopic = 11,
    RoadmapAddCheckpoint = 12,
    RoadmapMove = 13,
    RoadmapRemove = 14,
    ChallengeCreate = 20,
    ChallengeUpdate = 21,
    ChallengeClose = 22,
}

public enum ReviewStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,

    /// <summary>Aprovada, mas a alteração não pôde ser aplicada (o item mudou ou sumiu desde a proposta).</summary>
    Failed = 3,
}

/// <summary>
/// Alteração de conteúdo feita por um professor, esperando outro professor ou um admin aprovar.
/// A alteração fica guardada em <see cref="Payload"/> (JSON) e só é aplicada na aprovação.
/// </summary>
public class ContentReview
{
    public const int SummaryMaxLength = 300;
    public const int NoteMaxLength = 1000;

    public int Id { get; set; }
    public int TrackId { get; set; }
    public ReviewKind Kind { get; set; }

    /// <summary>Item alterado (questão, nó do roadmap ou desafio); nulo quando a proposta cria um item novo.</summary>
    public int? TargetId { get; set; }

    public required string Payload { get; set; }
    public required string Summary { get; set; }
    public required string AuthorId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ReviewStatus Status { get; set; }
    public string? ReviewerId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }

    /// <summary>Motivo da recusa, ou os erros quando a aplicação falhou.</summary>
    public string? Note { get; set; }

    /// <summary>Id do item criado ou alterado depois da aprovação.</summary>
    public int? ResultId { get; set; }
}

/// <summary>Título e texto de um nó do roadmap (editar, novo assunto, novo checkpoint).</summary>
public sealed record RoadmapTextPayload(string Title, string Summary);

public sealed record RoadmapMovePayload(bool Up);

public static class ReviewKindNames
{
    public static string Name(this ReviewKind kind) => kind switch
    {
        ReviewKind.QuestionCreate => "Nova questão",
        ReviewKind.QuestionUpdate => "Edição de questão",
        ReviewKind.QuestionArchive => "Arquivar questão",
        ReviewKind.RoadmapUpdate => "Edição do roadmap",
        ReviewKind.RoadmapAddTopic => "Novo assunto no roadmap",
        ReviewKind.RoadmapAddCheckpoint => "Novo checkpoint no roadmap",
        ReviewKind.RoadmapMove => "Reordenar o roadmap",
        ReviewKind.RoadmapRemove => "Remover do roadmap",
        ReviewKind.ChallengeCreate => "Novo desafio",
        ReviewKind.ChallengeUpdate => "Edição de desafio",
        ReviewKind.ChallengeClose => "Encerrar desafio",
        _ => kind.ToString(),
    };

    public static string Name(this ReviewStatus status) => status switch
    {
        ReviewStatus.Pending => "Aguardando revisão",
        ReviewStatus.Approved => "Aprovada",
        ReviewStatus.Rejected => "Recusada",
        ReviewStatus.Failed => "Aprovada, mas não aplicada",
        _ => status.ToString(),
    };
}
