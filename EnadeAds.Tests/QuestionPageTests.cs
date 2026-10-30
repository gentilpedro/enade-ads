using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class QuestionPageTests(EnadeAdsFactory factory)
{
    // Trechos que só existem nos comentários das alternativas da questão 1 (pilha).
    private const string CorrectExplanation = "Depois de push(1) e push(2)";
    private const string WrongExplanation = "Esse seria o resultado de uma fila";

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Visitor_SeesStatementButNeverTheAnswer()
    {
        var html = await GetHtmlAsync(NewClient(), "/questoes/1");

        Assert.Contains("push(1), push(2), pop()", html);
        Assert.Contains("1, 3, 5", html);
        Assert.Contains("para responder e ver o gabarito", html);
        Assert.DoesNotContain(CorrectExplanation, html);
        Assert.DoesNotContain(WrongExplanation, html);
        Assert.DoesNotContain("Correta", html);
        Assert.DoesNotContain("name=\"Letter\"", html);
    }

    [Fact]
    public async Task Visitor_CannotAnswer()
    {
        var before = await AttemptCountAsync(7);

        // Sem formulário para visitante: o token vem da página de login para simular um POST forjado.
        var response = await PostAnswerAsync(NewClient(), 7, "A", tokenSource: "/Account/Login");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(before, await AttemptCountAsync(7));
    }

    [Fact]
    public async Task List_IsOpenAndFiltersBySubject()
    {
        var client = NewClient();

        // O banco tem as questões do Inep além das de exemplo: o teste confere o filtro, não a contagem.
        var all = await GetHtmlAsync(client, "/questoes");
        var stacks = await GetHtmlAsync(client, "/questoes?materia=estruturas-dados");
        var sql = await GetHtmlAsync(client, "/questoes?materia=banco-de-dados");

        Assert.Contains("questões", all);
        Assert.Contains("push(1)", stacks);
        Assert.Contains("Considere as tabelas", sql);
        Assert.DoesNotContain("push(1)", sql);
    }

    [Fact]
    public async Task UnknownQuestion_Returns404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync("/questoes/9999")).StatusCode);
    }

    [Fact]
    public async Task Student_AnswersCorrectlyAndSeesAllExplanations()
    {
        var client = await LoggedClientAsync("questao.certa@gmail.com");

        var response = await PostAnswerAsync(client, 1, "A");
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Você acertou", html);
        Assert.Contains(CorrectExplanation, html);
        Assert.Contains(WrongExplanation, html);
    }

    [Fact]
    public async Task Student_AnswersWrongAndSeesTheCorrectLetter()
    {
        var client = await LoggedClientAsync("questao.errada@gmail.com");

        var html = WebUtility.HtmlDecode(await (await PostAnswerAsync(client, 1, "D")).Content.ReadAsStringAsync());

        Assert.Contains("Você marcou D", html);
        Assert.Contains("A resposta correta é a alternativa A", html);
        Assert.Contains("Sua resposta", html);
    }

    [Fact]
    public async Task Result_PersistsAndRetryShowsTheFormAgain()
    {
        var client = await LoggedClientAsync("questao.refazer@gmail.com");
        await PostAnswerAsync(client, 2, "B");

        var again = await GetHtmlAsync(client, "/questoes/2");
        var retry = await GetHtmlAsync(client, "/questoes/2?refazer=1");
        var list = await GetHtmlAsync(client, "/questoes?materia=arquitetura-computadores");

        Assert.Contains("Você marcou B", again);
        Assert.Contains("name=\"Letter\"", retry);
        Assert.DoesNotContain("Você marcou B", retry);
        Assert.Contains("Errou", list);
    }

    [Fact]
    public async Task InvalidLetter_IsRejectedAndNotSaved()
    {
        var client = await LoggedClientAsync("questao.invalida@gmail.com");
        var before = await AttemptCountAsync(3);

        var response = await PostAnswerAsync(client, 3, "Z");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await AttemptCountAsync(3));
    }

    private async Task<HttpClient> LoggedClientAsync(string email)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));

    private static async Task<HttpResponseMessage> PostAnswerAsync(HttpClient client, int questionId, string letter, string? tokenSource = null)
    {
        var url = $"/questoes/{questionId}";
        var page = await client.GetStringAsync(tokenSource ?? $"{url}?refazer=1");
        return await TestAccounts.PostFormAsync(client, url, page, "answer", new() { ["Letter"] = letter });
    }

    private async Task<int> AttemptCountAsync(int questionId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.QuestionAttempts.CountAsync(a => a.QuestionId == questionId);
    }
}
