using Microsoft.AspNetCore.Identity;

namespace EnadeAds.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string FullName { get; set; } = "";

    /// <summary>Quando a pessoa aceitou os termos de uso e a política de privacidade, e qual versão.</summary>
    public DateTimeOffset? TermsAcceptedAt { get; set; }

    public string? TermsVersion { get; set; }
}
