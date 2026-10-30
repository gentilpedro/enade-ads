using EnadeAds.Study;

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
    public int TrackId { get; set; } = Study.Tracks.Default;
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

/// <summary>
/// Quantas questões de cada parte o simulado pede, conforme o estilo da trilha.
/// ENADE (Portaria Inep nº 169/2026): 15 de formação geral + 30 específicas + 1 discursiva em 4 horas.
/// ENEM: 90 questões objetivas num dia de prova de 5 horas (o segundo dia), equilibradas entre as áreas;
/// a redação não entra no simulado. No ENEM, <see cref="Specific"/> são as objetivas.
/// </summary>
public sealed record MockExamFormat(MockExamStyle Style, MockExamSize Size, string Name, int GeneralEducation, int Specific, int Discursive)
{
    public static IReadOnlyList<MockExamFormat> All { get; } =
    [
        new(MockExamStyle.Enade, MockExamSize.Short, "Curto", 3, 7, 1),
        new(MockExamStyle.Enade, MockExamSize.Medium, "Médio", 7, 15, 1),
        new(MockExamStyle.Enade, MockExamSize.Full, "Completo, como a prova", 15, 30, 1),
        new(MockExamStyle.Enem, MockExamSize.Short, "Curto", 0, 20, 0),
        new(MockExamStyle.Enem, MockExamSize.Medium, "Médio", 0, 45, 0),
        new(MockExamStyle.Enem, MockExamSize.Full, "Completo, como um dia de prova", 0, 90, 0),
    ];

    public int Total => GeneralEducation + Specific + Discursive;

    public static IReadOnlyList<MockExamFormat> For(MockExamStyle style) => All.Where(f => f.Style == style).ToList();

    public static MockExamFormat Of(MockExamStyle style, MockExamSize size) => All.Single(f => f.Style == style && f.Size == size);

    /// <summary>Formato de um simulado já feito, pela trilha dele.</summary>
    public static MockExamFormat Of(MockExam exam) => Of(StyleOf(exam.TrackId), exam.Size);

    public static MockExamStyle StyleOf(int trackId) =>
        Tracks.Seed.FirstOrDefault(t => t.Id == trackId)?.MockExamStyle ?? MockExamStyle.Enade;

    /// <summary>Tempo proporcional ao da prova: 4 horas para 46 questões no ENADE, 5 horas para 90 no ENEM.</summary>
    public static int MinutesFor(MockExamStyle style, int questionCount)
    {
        var (minutes, questions) = style == MockExamStyle.Enem ? (300, 90) : (240, 46);
        return Math.Max(10, (int)Math.Round(minutes * questionCount / (double)questions));
    }
}
