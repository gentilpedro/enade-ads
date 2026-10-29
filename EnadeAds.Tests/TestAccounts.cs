using System.Net;
using System.Text.RegularExpressions;
using EnadeAds.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

public static partial class TestAccounts
{
    public const string Password = "Senha@Forte123";

    public static async Task<ApplicationUser> CreateConfirmedAsync(EnadeAdsFactory factory, string email, params string[] roles)
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = $"Teste {email}" };
        Ensure(await users.CreateAsync(user, Password));
        if (roles.Length > 0)
        {
            Ensure(await users.AddToRolesAsync(user, roles));
        }

        return user;
    }

    public static async Task<IList<string>> RolesOfAsync(EnadeAdsFactory factory, string email)
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await users.FindByEmailAsync(email) ?? throw new InvalidOperationException($"Conta {email} não existe.");
        return await users.GetRolesAsync(user);
    }

    /// <summary>Envia um formulário SSR do Blazor com o token antiforgery da própria página.</summary>
    public static async Task<HttpResponseMessage> SubmitFormAsync(HttpClient client, string url, string formName, Dictionary<string, string> fields)
    {
        var page = await client.GetStringAsync(url);
        var form = new Dictionary<string, string>(fields)
        {
            ["_handler"] = formName,
            ["__RequestVerificationToken"] = WebUtility.HtmlDecode(AntiforgeryToken().Match(page).Groups[1].Value),
        };

        return await client.PostAsync(url, new FormUrlEncodedContent(form));
    }

    public static async Task LoginAsync(HttpClient client, string email)
    {
        var response = await SubmitFormAsync(client, "/Account/Login", "login", new()
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password,
        });

        if (response.StatusCode != HttpStatusCode.Redirect)
        {
            throw new InvalidOperationException($"Login de {email} falhou: {response.StatusCode}.");
        }
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryToken();
}
