using System.Net;
using System.Text.RegularExpressions;
using EnadeAds.Accounts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public partial class TranslationTests(EnadeAdsFactory factory)
{
    // Palavras que só apareceriam se sobrasse texto em inglês do template do Identity.
    private static readonly string[] EnglishWords =
    [
        "Password", "Email", "Log in", "Login", "Register", "Manage", "Change", "Save", "Delete", "Enable", "Disable",
        "Reset", "Recovery", "Profile", "Personal", "Authenticator", "Error", "Confirm", "Submit", "Update", "Remove",
        "Forgot", "Resend", "Phone", "Username", "Locked", "Invalid", "Rename", "About", "Reload", "account",
    ];

    // Nomes de produtos citados nas instruções do autenticador: ficam como são.
    private static readonly string[] ProductNames = ["Microsoft Authenticator", "Google Authenticator"];

    public static TheoryData<string> PublicPages =>
    [
        "/Account/Login", "/Account/Register", "/Account/ForgotPassword", "/Account/ForgotPasswordConfirmation",
        "/Account/ResendEmailConfirmation", "/Account/Lockout", "/Account/InvalidPasswordReset", "/Account/InvalidUser",
        "/Account/ResetPasswordConfirmation", "/Account/ResetPassword?code=abc", "/pagina-que-nao-existe",
    ];

    public static TheoryData<string> AccountPages =>
    [
        "/Account/Manage", "/Account/Manage/Email", "/Account/Manage/ChangePassword", "/Account/Manage/TwoFactorAuthentication",
        "/Account/Manage/Passkeys", "/Account/Manage/PersonalData", "/Account/Manage/DeletePersonalData",
        "/Account/Manage/EnableAuthenticator", "/Account/Manage/ResetAuthenticator",
    ];

    [Theory]
    [MemberData(nameof(PublicPages))]
    public async Task PublicAccountPages_HaveNoEnglishText(string url)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync(url);

        AssertNoEnglish(url, await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [MemberData(nameof(AccountPages))]
    public async Task MyAccountPages_HaveNoEnglishText(string url)
    {
        var email = $"traducao.{Guid.NewGuid():N}@gmail.com";
        await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        await TestAccounts.LoginAsync(client, email);

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AssertNoEnglish(url, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task IdentityErrors_AreInPortugueseAndShownInRed()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        await TestAccounts.CreateConfirmedAsync(factory, "traducao.duplicada@gmail.com", Roles.Student);

        var duplicated = await RegisterAsync(client, "traducao.duplicada@gmail.com", TestAccounts.Password);
        var weak = await RegisterAsync(client, "traducao.fraca@gmail.com", "senhasemnumero");

        Assert.Contains("Já existe uma conta com o e-mail", duplicated);
        Assert.Contains("alert-danger", duplicated);
        Assert.Contains("A senha precisa ter pelo menos um número", weak);
        Assert.Contains("A senha precisa ter pelo menos uma letra maiúscula", weak);
    }

    [Fact]
    public async Task RequiredFieldMessages_AreInPortuguese()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await TestAccounts.SubmitFormAsync(client, "/Account/ForgotPassword", "forgot-password", new() { ["Input.Email"] = "" });
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Contains("Preencha este campo.", html);
        Assert.DoesNotContain("field is required", html);
    }

    private static async Task<string> RegisterAsync(HttpClient client, string email, string password)
    {
        var response = await TestAccounts.SubmitFormAsync(client, "/Account/Register", "register", new()
        {
            ["Input.FullName"] = "Pessoa Teste",
            ["Input.Email"] = email,
            ["Input.Password"] = password,
            ["Input.ConfirmPassword"] = password,
            ["Input.AcceptTerms"] = "true",
            ["Input.AgeGroup"] = "18+",
        });
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }

    private static void AssertNoEnglish(string url, string html)
    {
        // Só o texto visível: tira scripts, estilos e tags (atributos como autocomplete="current-password" não contam).
        var withoutScripts = ScriptsAndStyles().Replace(html, " ");
        var text = WebUtility.HtmlDecode(Tags().Replace(withoutScripts, " "));
        text = ProductNames.Aggregate(text, (current, name) => current.Replace(name, " "));

        var found = EnglishWords.Where(word => Regex.IsMatch(text, $@"(?<![\p{{L}}-]){Regex.Escape(word)}(?![\p{{L}}-])")).ToList();
        Assert.True(found.Count == 0, $"{url} ainda tem texto em inglês: {string.Join(", ", found)}");
    }

    [GeneratedRegex(@"<(script|style)[^>]*>.*?</\1>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ScriptsAndStyles();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Tags();
}
