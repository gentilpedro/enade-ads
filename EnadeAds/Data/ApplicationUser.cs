using Microsoft.AspNetCore.Identity;

namespace EnadeAds.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string FullName { get; set; } = "";
}
