namespace EnadeAds.Study;

public sealed class ExamOptions
{
    public const string SectionName = "Enade";

    // As datas de prova ficam em cada trilha (tabela Tracks), não aqui.

    /// <summary>Fuso usado para saber que dia é "hoje" para quem faz a prova.</summary>
    public string TimeZone { get; set; } = "America/Sao_Paulo";
}
