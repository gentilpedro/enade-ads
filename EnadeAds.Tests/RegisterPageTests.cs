using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class RegisterPageTests(EnadeAdsFactory factory)
{
    private readonly HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task LoginPage_IsPublic()
    {
        var response = await client.GetAsync("/Account/Login");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Entrar", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ManagePage_RedirectsAnonymousUserToLogin()
    {
        var response = await client.GetAsync("/Account/Manage");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task RegisterPage_ExplainsTeacherCheckbox()
    {
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/Account/Register"));

        Assert.Contains("Sou professor", html);
        Assert.Contains("pendente", html);
    }

    [Fact]
    public async Task Register_Student_CreatesStudentAccount()
    {
        var response = await RegisterAsync("aluna@gmail.com", wantsTeacher: false);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal([Roles.Student], await RolesOfAsync("aluna@gmail.com"));
    }

    [Fact]
    public async Task Register_TeacherWithEduEmail_IsPending()
    {
        var response = await RegisterAsync("prof@faculdade.edu.br", wantsTeacher: true);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal([Roles.Student, Roles.PendingTeacher], (await RolesOfAsync("prof@faculdade.edu.br")).Order());
    }

    [Fact]
    public async Task Register_TeacherWithoutEduEmail_IsRejected()
    {
        var response = await RegisterAsync("prof@gmail.com", wantsTeacher: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("e-mail institucional", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        Assert.Null(await FindUserAsync("prof@gmail.com"));
    }

    [Fact]
    public async Task Register_ConfiguredAdmin_GetsAdminRole()
    {
        await RegisterAsync(EnadeAdsFactory.AdminEmail, wantsTeacher: false);

        Assert.Contains(Roles.Admin, await RolesOfAsync(EnadeAdsFactory.AdminEmail));
    }

    private async Task<HttpResponseMessage> RegisterAsync(string email, bool wantsTeacher)
    {
        var fields = new Dictionary<string, string>
        {
            ["Input.FullName"] = "Pessoa Teste",
            ["Input.Email"] = email,
            ["Input.Password"] = TestAccounts.Password,
            ["Input.ConfirmPassword"] = TestAccounts.Password,
            ["Input.AcceptTerms"] = "true",
            ["Input.AgeGroup"] = "18+",
        };
        if (wantsTeacher)
        {
            fields["Input.WantsTeacherAccess"] = "true";
        }

        return await TestAccounts.SubmitFormAsync(client, "/Account/Register", "register", fields);
    }

    private async Task<ApplicationUser?> FindUserAsync(string email)
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await users.FindByEmailAsync(email);
    }

    private Task<IList<string>> RolesOfAsync(string email) => TestAccounts.RolesOfAsync(factory, email);
}
