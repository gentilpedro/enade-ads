namespace EnadeAds.MockExams;

public enum MockExamSize
{
    Short = 0,
    Medium = 1,
    Full = 2,
}

/// <summary>Simulado de um aluno. O prazo é gravado no início: recarregar a página não reinicia o relógio.</summary>
public class MockExam
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public MockExamSize Size { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset DeadlineAt { get; set; }

    /// <summary>Preenchido na entrega, pelo aluno ou automaticamente quando o tempo acaba.</summary>
    public DateTimeOffset? SubmittedAt { get; set; }

    public bool SubmittedByTimeout { get; set; }
    public int ObjectiveCount { get; set; }
    public int? CorrectCount { get; set; }

    public List<MockExamQuestion> Questions { get; set; } = [];

    public bool IsOpen => SubmittedAt is null;
}

public class MockExamQuestion
{
    public int MockExamId { get; set; }
    public int QuestionId { get; set; }
    public Questions.Question? Question { get; set; }
    public int Order { get; set; }

    /// <summary>Formação geral ou componente específico, como na prova.</summary>
    public bool IsGeneralEducation { get; set; }

    public int? SelectedOptionId { get; set; }
    public string? DiscursiveText { get; set; }
}

/// <summary>Quantas questões de cada parte o simulado pede (Portaria Inep nº 169/2026: 15 + 30 + 1 discursiva em 4 horas).</summary>
public sealed record MockExamFormat(MockExamSize Size, string Name, int GeneralEducation, int Specific, int Discursive)
{
    public const int FullMinutes = 240;
    public const int FullQuestionCount = 46;

    public static IReadOnlyList<MockExamFormat> All { get; } =
    [
        new(MockExamSize.Short, "Curto", 3, 7, 1),
        new(MockExamSize.Medium, "Médio", 7, 15, 1),
        new(MockExamSize.Full, "Completo, como a prova", 15, 30, 1),
    ];

    public int Total => GeneralEducation + Specific + Discursive;

    public static MockExamFormat Of(MockExamSize size) => All.Single(f => f.Size == size);

    /// <summary>Tempo proporcional ao da prova: 4 horas para 46 questões.</summary>
    public static int MinutesFor(int questionCount) =>
        Math.Max(10, (int)Math.Round(FullMinutes * questionCount / (double)FullQuestionCount));
}
