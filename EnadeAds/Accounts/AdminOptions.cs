namespace EnadeAds.Accounts;

public sealed class AdminOptions
{
    public const string SectionName = "Admin";

    /// <summary>E-mails que recebem o perfil Admin ao se cadastrar.</summary>
    public List<string> Emails { get; set; } = [];
}
