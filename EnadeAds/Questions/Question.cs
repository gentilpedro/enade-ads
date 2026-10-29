using EnadeAds.Study;

namespace EnadeAds.Questions;

/// <summary>Questão objetiva no formato do ENADE: enunciado e cinco alternativas, uma correta.</summary>
public class Question
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public RoadmapNode? Subject { get; set; }

    /// <summary>Texto puro; quebras de linha e indentação são preservadas (código e tabelas).</summary>
    public required string Statement { get; set; }

    /// <summary>De onde vem a questão, por exemplo "ENADE 2021 – questão 12" ou "Autoria própria".</summary>
    public required string Source { get; set; }

    /// <summary>Autor na plataforma; nulo para o conteúdo inicial.</summary>
    public string? AuthorId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<QuestionOption> Options { get; set; } = [];
}

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public char Letter { get; set; }
    public required string Text { get; set; }
    public bool IsCorrect { get; set; }

    /// <summary>Por que a alternativa está certa ou errada; só aparece depois de responder.</summary>
    public required string Explanation { get; set; }
}

public class QuestionAttempt
{
    public long Id { get; set; }
    public int QuestionId { get; set; }
    public required string UserId { get; set; }
    public int OptionId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTimeOffset AnsweredAt { get; set; }
}
