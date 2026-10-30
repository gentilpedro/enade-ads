using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class QuestionFormTests(EnadeAdsFactory factory)
{
    // IHC não tem questões de exemplo: as questões criadas aqui não mexem nas contagens de outros testes.
    private const int SubjectId = 1400;

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory]
    [InlineData("form.aluno@gmail.com", Roles.Student)]
    [InlineData("form.pendente@faculdade.edu.br", Roles.PendingTeacher)]
    public async Task OnlyTeachersAndAdminsOpenTheForm(string email, string role)
    {
        var client = await LoggedClientAsync(email, Roles.Student, role);

        var response = await client.GetAsync("/questoes/nova");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/AccessDenied", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Teacher_PublishesQuestion()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "form.publica@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, teacher.Email!);

        var response = await SubmitAsync(client, "/questoes/nova", Fields("Qual comando lista tabelas no **PostgreSQL**?", 'B'), "publicar");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var id = int.Parse(response.Headers.Location!.ToString().Split('/')[^1]);
        var question = await FindAsync(id);
        Assert.Equal(teacher.Id, question.AuthorId);
        Assert.Equal(SubjectId, question.SubjectId);
        Assert.Equal('B', Assert.Single(question.Options, o => o.IsCorrect).Letter);

        var page = WebUtility.HtmlDecode(await client.GetStringAsync($"/questoes/{id}"));
        Assert.Contains("<strong>PostgreSQL</strong>", page);
        Assert.Contains("Editar questão", page);
    }

    [Fact]
    public async Task Preview_ShowsTheQuestionWithoutSaving()
    {
        var client = await LoggedClientAsync("form.previa@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var before = await CountAsync();

        var response = await SubmitAsync(client, "/questoes/nova", Fields("Enunciado em pré-visualização", 'C'), "pre-visualizar");
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Pré-visualização", html);
        Assert.Contains("Enunciado em pré-visualização", html);
        Assert.Equal(before, await CountAsync());
    }

    [Fact]
    public async Task InvalidQuestion_IsNotSaved()
    {
        var client = await LoggedClientAsync("form.invalida@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var before = await CountAsync();
        var fields = Fields("", 'A');
        fields.Remove("Input.CorrectLetter");

        var response = await SubmitAsync(client, "/questoes/nova", fields, "publicar");
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Preencha o enunciado.", html);
        Assert.Contains("Marque qual alternativa é a correta.", html);
        Assert.Equal(before, await CountAsync());
    }

    [Fact]
    public async Task Teacher_CannotEditSomeoneElsesQuestion()
    {
        var author = await TestAccounts.CreateConfirmedAsync(factory, "form.autor@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var id = await CreateWithServiceAsync(author.Id);
        var other = await LoggedClientAsync("form.outro@faculdade.edu.br", Roles.Student, Roles.Teacher);

        var page = await other.GetAsync($"/questoes/{id}/editar");
        var detail = WebUtility.HtmlDecode(await other.GetStringAsync($"/questoes/{id}"));

        Assert.NotEqual(HttpStatusCode.OK, page.StatusCode);
        Assert.DoesNotContain("Editar questão", detail);

        using var scope = factory.Services.CreateScope();
        var editor = scope.ServiceProvider.GetRequiredService<QuestionEditor>();
        var forged = await editor.UpdateAsync(new Editor("qualquer", IsAdmin: false, IsTeacher: true), id, Draft("Trocado", 'A'));
        Assert.False(forged.Succeeded);
    }

    [Fact]
    public async Task Admin_EditsAnyQuestion()
    {
        var author = await TestAccounts.CreateConfirmedAsync(factory, "form.autor2@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var id = await CreateWithServiceAsync(author.Id);

        using var scope = factory.Services.CreateScope();
        var editor = scope.ServiceProvider.GetRequiredService<QuestionEditor>();
        var result = await editor.UpdateAsync(new Editor("admin", IsAdmin: true, IsTeacher: false), id, Draft("Enunciado revisado pelo admin", 'E'));

        Assert.True(result.Succeeded);
        var question = await FindAsync(id);
        Assert.Equal("Enunciado revisado pelo admin", question.Statement);
        Assert.Equal('E', Assert.Single(question.Options, o => o.IsCorrect).Letter);
    }

    [Fact]
    public async Task Archive_HidesQuestionButKeepsAttempts()
    {
        var author = await TestAccounts.CreateConfirmedAsync(factory, "form.arquiva@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var id = await CreateWithServiceAsync(author.Id);
        var student = await TestAccounts.CreateConfirmedAsync(factory, "form.respondeu@gmail.com", Roles.Student);

        using (var scope = factory.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<QuestionBank>().AnswerAsync(student.Id, id, 'A');
        }

        var client = NewClient();
        await TestAccounts.LoginAsync(client, author.Email!);
        var response = await TestAccounts.SubmitFormAsync(client, $"/questoes/{id}/editar", "archive-form", new() { ["ConfirmArchive"] = "true" });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull((await FindAsync(id)).ArchivedAt);
        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync($"/questoes/{id}")).StatusCode);
        Assert.Contains("está arquivada", WebUtility.HtmlDecode(await client.GetStringAsync($"/questoes/{id}/editar")));
        Assert.Equal(1, await AttemptCountAsync(id));
    }

    private async Task<HttpClient> LoggedClientAsync(string email, params string[] roles)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, roles);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }

    private static Task<HttpResponseMessage> SubmitAsync(HttpClient client, string url, Dictionary<string, string> fields, string command)
    {
        fields["Command"] = command;
        return TestAccounts.SubmitFormAsync(client, url, "question-form", fields);
    }

    private static Dictionary<string, string> Fields(string statement, char correct)
    {
        var fields = new Dictionary<string, string>
        {
            ["Input.SubjectId"] = SubjectId.ToString(),
            ["Input.Source"] = "Autoria própria",
            ["Input.Statement"] = statement,
            ["Input.CorrectLetter"] = correct.ToString(),
        };
        for (var i = 0; i < QuestionDraft.OptionCount; i++)
        {
            fields[$"Input.Options[{i}].Text"] = $"Alternativa {(char)('A' + i)}";
            fields[$"Input.Options[{i}].Explanation"] = $"Comentário {(char)('A' + i)}";
        }

        return fields;
    }

    private static QuestionDraft Draft(string statement, char correct) => new()
    {
        SubjectId = SubjectId,
        Statement = statement,
        Source = "Autoria própria",
        CorrectLetter = correct,
        Options = Enumerable.Range(0, QuestionDraft.OptionCount)
            .Select(i => new OptionDraft { Text = $"Alternativa {i}", Explanation = $"Comentário {i}" }).ToList(),
    };

    private async Task<int> CreateWithServiceAsync(string authorId)
    {
        using var scope = factory.Services.CreateScope();
        var editor = scope.ServiceProvider.GetRequiredService<QuestionEditor>();
        var result = await editor.CreateAsync(new Editor(authorId, IsAdmin: false, IsTeacher: true), Draft("Questão do autor", 'A'));
        return result.Id!.Value;
    }

    private async Task<Question> FindAsync(int id)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.Questions.Include(q => q.Options).SingleAsync(q => q.Id == id);
    }

    private async Task<int> CountAsync()
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Questions.CountAsync();
    }

    private async Task<int> AttemptCountAsync(int questionId)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().QuestionAttempts.CountAsync(a => a.QuestionId == questionId);
    }
}
