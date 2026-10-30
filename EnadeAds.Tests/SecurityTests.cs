using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

public class AccountRateLimitTests
{
    private static readonly RateLimitOptions Options = new() { AccountPostsPerMinute = 10 };

    [Fact]
    public void AllowsTenPostsPerMinuteFromTheSameIpThenRejects()
    {
        using var limiter = AccountRateLimit.Create(Options);

        var results = Enumerable.Range(0, 11)
            .Select(_ => { using var lease = limiter.AttemptAcquire(Request("POST", "/Account/Login", "200.1.1.1")); return lease.IsAcquired; })
            .ToList();

        Assert.All(results.Take(10), Assert.True);
        Assert.False(results[^1]);
    }

    [Fact]
    public void EachIpHasItsOwnLimit()
    {
        using var limiter = AccountRateLimit.Create(Options);
        for (var i = 0; i < 10; i++)
        {
            limiter.AttemptAcquire(Request("POST", "/Account/Register", "200.2.2.2")).Dispose();
        }

        using var otherIp = limiter.AttemptAcquire(Request("POST", "/Account/Register", "200.3.3.3"));

        Assert.True(otherIp.IsAcquired);
    }

    [Theory]
    [InlineData("GET", "/Account/Login")]
    [InlineData("POST", "/questoes/1")]
    [InlineData("POST", "/blog/algum-post")]
    public void OnlyAccountFormPostsAreLimited(string method, string path)
    {
        using var limiter = AccountRateLimit.Create(Options);

        var results = Enumerable.Range(0, 50)
            .Select(_ => { using var lease = limiter.AttemptAcquire(Request(method, path, "200.4.4.4")); return lease.IsAcquired; });

        Assert.All(results, Assert.True);
    }

    [Theory]
    [InlineData("/Account/Login")]
    [InlineData("/account/register")]
    [InlineData("/Account/ForgotPassword")]
    [InlineData("/Account/ResendEmailConfirmation")]
    [InlineData("/Account/ResetPassword")]
    [InlineData("/Account/LoginWith2fa")]
    [InlineData("/Account/LoginWithRecoveryCode")]
    public void ProtectsTheBruteForceTargets(string path)
    {
        Assert.True(AccountRateLimit.IsProtected(Request("POST", path, "200.5.5.5")));
    }

    private static DefaultHttpContext Request(string method, string path, string ip)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        return context;
    }
}

[Collection(AppCollection.Name)]
public class SecurityTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task FiveWrongPasswordsLockTheAccount()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "seguranca.forca.bruta@gmail.com", Roles.Student);

        var redirects = new List<string>();
        for (var attempt = 0; attempt < 5; attempt++)
        {
            redirects.Add(await LoginAsync(student.Email!, "SenhaErrada123"));
        }

        var withRightPassword = await LoginAsync(student.Email!, TestAccounts.Password);

        Assert.All(redirects.Take(4), location => Assert.Equal("", location));
        Assert.Contains("/Account/Lockout", redirects[^1]);
        Assert.Contains("/Account/Lockout", withRightPassword);
    }

    [Fact]
    public async Task AdminUnblockClearsTheTemporaryLockout()
    {
        var admin = await TestAccounts.CreateConfirmedAsync(factory, "seguranca.admin@faculdade.edu.br", Roles.Student, Roles.Admin);
        var student = await TestAccounts.CreateConfirmedAsync(factory, "seguranca.desbloqueio@gmail.com", Roles.Student);
        for (var attempt = 0; attempt < 5; attempt++)
        {
            await LoginAsync(student.Email!, "SenhaErrada123");
        }

        using (var scope = factory.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AccountAdmin>().ApplyAsync(admin.Id, student.Id, AccountAction.Unblock);
        }

        Assert.DoesNotContain("/Account/Lockout", await LoginAsync(student.Email!, TestAccounts.Password));
    }

    [Fact]
    public async Task EveryResponseCarriesSecurityHeaders()
    {
        var response = await NewClient().GetAsync("/");
        var csp = response.Headers.GetValues("Content-Security-Policy").Single();

        Assert.Contains("frame-ancestors 'none'", csp);
        Assert.Contains("object-src 'none'", csp);
        Assert.Contains("form-action 'self'", csp);
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Equal("strict-origin-when-cross-origin", response.Headers.GetValues("Referrer-Policy").Single());
    }

    /// <summary>Tenta entrar e devolve para onde o login redirecionou ("" quando a tela volta com erro).</summary>
    private async Task<string> LoginAsync(string email, string password)
    {
        var response = await TestAccounts.SubmitFormAsync(NewClient(), "/Account/Login", "login", new()
        {
            ["Input.Email"] = email,
            ["Input.Password"] = password,
        });
        return response.Headers.Location?.ToString() ?? "";
    }
}
