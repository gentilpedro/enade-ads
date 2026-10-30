using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class DiscursiveQuestionTests(EnadeAdsFactory factory)
{
    private const int DiscursiveId = 8;

    // Trecho que só existe no padrão de resposta da questão 8.
    private const string ExpectedAnswerExcerpt = "Perde pontos a resposta que";

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Visitor_SeesStatementButNotTheExpectedAnswer()
    {
        var html = await GetHtmlAsync(NewClient(), $"/questoes/{DiscursiveId}");

        Assert.Contains("rematrícula pelo celular", html);
        Assert.Contains("Discursiva", html);
        Assert.Contains("ver o padrão de resposta", html);
        Assert.DoesNotContain(ExpectedAnswerExcerpt, html);
        Assert.DoesNotContain("name=\"AnswerText\"", html);
    }

    [Fact]
    public async Task Student_SeesExpectedAnswerOnlyAfterSending()
    {
        var client = await LoggedClientAsync("discursiva.envia@gmail.com");

        var before = await GetHtmlAsync(client, $"/questoes/{DiscursiveId}");
        var response = await PostAnswerAsync(client, "Requisito de desempenho com teste de carga.\nRequisito de acessibilidade com WCAG.");
        var after = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Contains("name=\"AnswerText\"", before);
        Assert.DoesNotContain(ExpectedAnswerExcerpt, before);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(ExpectedAnswerExcerpt, after);
        Assert.Contains("Requisito de desempenho com teste de carga.", after);
    }

    [Fact]
    public async Task StudentAnswer_IsShownAsTextNeverAsHtml()
    {
        var client = await LoggedClientAsync("discursiva.xss@gmail.com");

        var response = await PostAnswerAsync(client, "<script>alert('x')</script> e <b>negrito</b>");
        var raw = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("<script>alert", raw);
        Assert.DoesNotContain("<b>negrito</b>", raw);
        Assert.Contains("&lt;script&gt;", raw);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task EmptyAnswer_IsRejected(string text)
    {
        var client = await LoggedClientAsync($"discursiva.vazia{text.Length}@gmail.com");

        var response = await PostAnswerAsync(client, text);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TooLongAnswer_IsRejected()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "discursiva.longa@gmail.com", Roles.Student);

        using var scope = factory.Services.CreateScope();
        var bank = scope.ServiceProvider.GetRequiredService<QuestionBank>();

        Assert.Null(await bank.AnswerDiscursiveAsync(student.Id, DiscursiveId, new string('x', QuestionBank.DiscursiveAnswerMaxLength + 1)));
        Assert.NotNull(await bank.AnswerDiscursiveAsync(student.Id, DiscursiveId, new string('x', QuestionBank.DiscursiveAnswerMaxLength)));
    }

    [Fact]
    public async Task Answer_IsPrivateToItsAuthor()
    {
        var author = await LoggedClientAsync("discursiva.autora@gmail.com");
        await PostAnswerAsync(author, "Resposta particular da autora");
        var other = await LoggedClientAsync("discursiva.outra@gmail.com");

        var html = await GetHtmlAsync(other, $"/questoes/{DiscursiveId}");

        Assert.DoesNotContain("Resposta particular da autora", html);
        Assert.Contains("name=\"AnswerText\"", html);
    }

    [Fact]
    public async Task List_ShowsAnsweredAndRetryReopensTheForm()
    {
        var client = await LoggedClientAsync("discursiva.lista@gmail.com");
        await PostAnswerAsync(client, "Primeira versão");

        var list = await GetHtmlAsync(client, "/questoes?materia=engenharia-requisitos");
        var retry = await GetHtmlAsync(client, $"/questoes/{DiscursiveId}?refazer=1");

        Assert.Contains("Respondida", list);
        Assert.Contains("name=\"AnswerText\"", retry);
    }

    [Fact]
    public async Task ObjectiveAndDiscursivePathsDoNotMix()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "discursiva.mistura@gmail.com", Roles.Student);

        using var scope = factory.Services.CreateScope();
        var bank = scope.ServiceProvider.GetRequiredService<QuestionBank>();

        Assert.Null(await bank.AnswerAsync(student.Id, DiscursiveId, 'A'));
        Assert.Null(await bank.AnswerDiscursiveAsync(student.Id, 1, "Texto numa objetiva"));
    }

    [Fact]
    public async Task Teacher_CreatesDiscursiveWithoutOptions()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "discursiva.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, teacher.Email!);

        var response = await TestAccounts.SubmitFormAsync(client, "/questoes/nova", "question-form", new()
        {
            ["Input.Kind"] = nameof(QuestionKind.Discursive),
            ["Input.SubjectId"] = "1400",
            ["Input.Source"] = "Autoria própria",
            ["Input.Statement"] = "Proponha uma avaliação heurística para um app bancário.",
            ["Input.ExpectedAnswer"] = "Citar heurísticas de Nielsen e como aplicar.",
            ["Input.SuggestedLines"] = "12",
            ["Command"] = "publicar",
        });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var id = (await TestReviews.ApproveFromRedirectAsync(factory, response))!.Value;

        using var scope = factory.Services.CreateScope();
        var question = await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Questions
            .Include(q => q.Options).SingleAsync(q => q.Id == id);
        Assert.Equal(QuestionKind.Discursive, question.Kind);
        Assert.Empty(question.Options);
        Assert.Equal(12, question.SuggestedLines);
    }

    [Fact]
    public async Task Teacher_EditsDiscursiveThroughTheForm()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "discursiva.edita@faculdade.edu.br", Roles.Student, Roles.Teacher);
        int id;
        using (var scope = factory.Services.CreateScope())
        {
            var created = await scope.ServiceProvider.GetRequiredService<QuestionEditor>()
                .CreateAsync(new Editor(teacher.Id, IsAdmin: false, IsTeacher: true, Reviewed: true), Discursive(expectedAnswer: "Padrão antigo"));
            id = created.Id!.Value;
        }

        var client = NewClient();
        await TestAccounts.LoginAsync(client, teacher.Email!);
        var page = await GetHtmlAsync(client, $"/questoes/{id}/editar");

        // A edição de discursiva não envia alternativas: a seção nem aparece na tela.
        var response = await TestAccounts.SubmitFormAsync(client, $"/questoes/{id}/editar", "question-form", new()
        {
            ["Input.Kind"] = nameof(QuestionKind.Discursive),
            ["Input.SubjectId"] = "1400",
            ["Input.Source"] = "Autoria própria",
            ["Input.Statement"] = "Enunciado revisado",
            ["Input.ExpectedAnswer"] = "Padrão revisado",
            ["Input.SuggestedLines"] = "20",
            ["Command"] = "publicar",
        });

        Assert.DoesNotContain("Alternativas", page);
        Assert.Contains("Padrão de resposta", page);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        await TestReviews.ApproveFromRedirectAsync(factory, response);
        using var check = factory.Services.CreateScope();
        var question = await check.ServiceProvider.GetRequiredService<ApplicationDbContext>().Questions.SingleAsync(q => q.Id == id);
        Assert.Equal("Padrão revisado", question.ExpectedAnswer);
        Assert.Equal(20, question.SuggestedLines);
    }

    [Fact]
    public async Task Editor_RequiresExpectedAnswerAndKeepsKind()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "discursiva.regra@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var editor = new Editor(teacher.Id, IsAdmin: false, IsTeacher: true, Reviewed: true);

        using var scope = factory.Services.CreateScope();
        var questions = scope.ServiceProvider.GetRequiredService<QuestionEditor>();

        var missing = await questions.CreateAsync(editor, Discursive(expectedAnswer: ""));
        var created = await questions.CreateAsync(editor, Discursive(expectedAnswer: "Padrão"));
        var objectiveVersion = Discursive(expectedAnswer: "Padrão");
        objectiveVersion.Kind = QuestionKind.Objective;
        var changedKind = await questions.UpdateAsync(editor, created.Id!.Value, objectiveVersion);

        Assert.Contains("Preencha o padrão de resposta.", missing.Errors);
        Assert.True(created.Succeeded);
        Assert.False(changedKind.Succeeded);
        Assert.Contains("não muda depois de publicada", Assert.Single(changedKind.Errors));
    }

    private static QuestionDraft Discursive(string expectedAnswer) => new()
    {
        Kind = QuestionKind.Discursive,
        SubjectId = 1400,
        Statement = "Enunciado discursivo",
        Source = "Autoria própria",
        ExpectedAnswer = expectedAnswer,
        SuggestedLines = 15,
    };

    private async Task<HttpClient> LoggedClientAsync(string email)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));

    private static async Task<HttpResponseMessage> PostAnswerAsync(HttpClient client, string text)
    {
        var url = $"/questoes/{DiscursiveId}";
        var page = await client.GetStringAsync($"{url}?refazer=1");
        return await TestAccounts.PostFormAsync(client, url, page, "discursive-answer", new() { ["AnswerText"] = text });
    }
}
