using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EnadeAds.Tests;

/// <summary>Depois de enviar um formulário de conta, a pessoa vê o que aconteceu e tem como voltar ao início.</summary>
[Collection(AppCollection.Name)]
public class AccountFeedbackTests(EnadeAdsFactory factory)
{
    private readonly HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task ForgotPassword_EndsOnEmailSentPage()
    {
        var response = await TestAccounts.SubmitFormAsync(client, "/Account/ForgotPassword", "forgot-password", new()
        {
            ["Input.Email"] = "quem.esqueceu@gmail.com",
        });
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.EndsWith("/Account/ForgotPasswordConfirmation", response.Headers.Location?.ToString());

        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/Account/ForgotPasswordConfirmation"));

        Assert.Contains("<h1>E-mail enviado</h1>", html);
        Assert.Contains("alert-success", html);
        Assert.Contains("Voltar para o início", html);
    }

    [Fact]
    public async Task RegisterConfirmation_SaysTheAccountWasCreated()
    {
        await TestAccounts.CreateConfirmedAsync(factory, "cadastro.concluido@gmail.com");

        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/Account/RegisterConfirmation?email=cadastro.concluido%40gmail.com"));

        Assert.Contains("<h1>Cadastro concluído</h1>", html);
        Assert.Contains("Voltar para o início", html);
    }

    [Theory]
    [InlineData("/Account/Login")]
    [InlineData("/Account/Register")]
    [InlineData("/Account/ForgotPassword")]
    [InlineData("/Account/ResendEmailConfirmation")]
    [InlineData("/Account/ResetPasswordConfirmation")]
    public async Task AccountPages_LinkBackHome(string url)
    {
        var html = WebUtility.HtmlDecode(await client.GetStringAsync(url));

        Assert.Contains("Voltar para o início", html);
    }
}
