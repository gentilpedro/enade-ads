namespace EnadeAds.Study;

public sealed class ExamOptions
{
    public const string SectionName = "Enade";

    /// <summary>Data da prova (Edital Inep nº 49/2026: 29/11/2026).</summary>
    public DateOnly ExamDate { get; set; } = new(2026, 11, 29);

    /// <summary>Fuso usado para saber que dia é "hoje" para quem faz a prova.</summary>
    public string TimeZone { get; set; } = "America/Sao_Paulo";
}
