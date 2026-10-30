using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Challenges;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class ChallengeTests(EnadeAdsFactory factory)
{
    // Questões objetivas da seed: 1 (pilha, correta A), 2 (bases, correta A), 5 (requisitos, correta C).
    private static readonly int[] SeedQuestions = [1, 2, 5];

    // Trecho que só existe no comentário da alternativa correta da questão 1.
    private const string Question1Explanation = "Depois de push(1) e push(2)";

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Visitor_SeesActiveChallengeWithoutAnswersOrForm()
    {
        var id = await CreateAsync("Visitante vê", startOffset: -1, endOffset: 5);

        var html = await GetHtmlAsync(NewClient(), $"/desafios/{id}");

        Assert.Contains("Visitante vê", html);
        Assert.Contains("push(1), push(2), pop()", html);
        Assert.Contains("para participar do desafio", html);
        Assert.DoesNotContain(Question1Explanation, html);
        Assert.DoesNotContain("challenge-answers", html);
    }

    [Fact]
    public async Task UpcomingChallenge_IsHiddenFromStudentsButVisibleToItsAuthor()
    {
        var (author, _) = await TeacherAsync("desafio.agenda@faculdade.edu.br");
        var id = await CreateAsync("Agendado", startOffset: 3, endOffset: 9, authorId: author.Id);
        var student = await StudentClientAsync("desafio.curioso@gmail.com");
        var teacher = NewClient();
        await TestAccounts.LoginAsync(teacher, author.Email!);

        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync($"/desafios/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await student.GetAsync($"/desafios/{id}")).StatusCode);
        Assert.DoesNotContain("Agendado", await GetHtmlAsync(student, "/desafios"));
        Assert.Contains("Este desafio começa em", await GetHtmlAsync(teacher, $"/desafios/{id}"));
    }

    [Fact]
    public async Task Student_SubmitsOnceAndSeesScoreButNotTheAnswerKey()
    {
        var id = await CreateAsync("Envio único", startOffset: 0, endOffset: 0);
        var client = await StudentClientAsync("desafio.envia@gmail.com");

        var first = await SubmitAsync(client, id, new() { [1] = 'A', [2] = 'B', [5] = 'C' });
        var firstHtml = WebUtility.HtmlDecode(await first.Content.ReadAsStringAsync());
        var second = await SubmitAsync(client, id, new() { [1] = 'A', [2] = 'A', [5] = 'C' }, tokenPage: "/Account/Login");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Contains("Você acertou 2 de 3", firstHtml);
        Assert.DoesNotContain(Question1Explanation, firstHtml);
        Assert.False(second.IsSuccessStatusCode);
        Assert.Equal(1, await SubmissionCountAsync(id));
    }

    [Fact]
    public async Task IncompleteSubmission_IsRejected()
    {
        var id = await CreateAsync("Incompleto", startOffset: -1, endOffset: 1);
        var client = await StudentClientAsync("desafio.incompleto@gmail.com");

        var response = await SubmitAsync(client, id, new() { [1] = 'A' });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Responda todas as questões", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        Assert.Equal(0, await SubmissionCountAsync(id));
    }

    [Fact]
    public async Task ChallengeAnswers_DoNotUnlockTheQuestionPage()
    {
        var id = await CreateAsync("Separado do banco", startOffset: -1, endOffset: 1);
        var client = await StudentClientAsync("desafio.separado@gmail.com");
        await SubmitAsync(client, id, new() { [1] = 'A', [2] = 'A', [5] = 'C' });

        var questionPage = await GetHtmlAsync(client, "/questoes/1");

        Assert.Contains("name=\"Letter\"", questionPage);
        Assert.DoesNotContain(Question1Explanation, questionPage);
    }

    [Fact]
    public async Task EndedChallenge_ShowsAnswerKeyAndClassScoreboard()
    {
        var id = await CreateAsync("Encerrado", startOffset: -1, endOffset: 1);
        var first = await StudentClientAsync("desafio.placar1@gmail.com");
        var second = await StudentClientAsync("desafio.placar2@gmail.com");
        await SubmitAsync(first, id, new() { [1] = 'A', [2] = 'A', [5] = 'C' });
        await SubmitAsync(second, id, new() { [1] = 'D', [2] = 'A', [5] = 'C' });
        await CloseWithServiceAsync(id);

        var html = await GetHtmlAsync(second, $"/desafios/{id}");
        using var scope = factory.Services.CreateScope();
        var scoreboard = await scope.ServiceProvider.GetRequiredService<ChallengeService>().ScoreboardAsync(id);

        Assert.Contains("Desempenho da turma", html);
        Assert.Contains("2 alunos participaram", html);
        Assert.Contains("Você acertou 2 de 3", html);
        Assert.Contains(Question1Explanation, html);
        Assert.Contains("Sua resposta", html);
        Assert.Equal([50, 100, 100], scoreboard.Questions.Select(q => q.Percent));
    }

    [Fact]
    public async Task EndedChallenge_DoesNotAcceptAnswers()
    {
        var id = await CreateAsync("Fora do prazo", startOffset: -10, endOffset: -3);
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desafio.atrasado@gmail.com", Roles.Student);

        using var scope = factory.Services.CreateScope();
        var result = await scope.ServiceProvider.GetRequiredService<ChallengeService>()
            .SubmitAsync(student.Id, id, new Dictionary<int, char> { [1] = 'A', [2] = 'A', [5] = 'C' });

        Assert.Equal(SubmitOutcome.NotActive, result.Outcome);
    }

    [Fact]
    public async Task OnlyAuthorOrAdminCloses()
    {
        var (author, _) = await TeacherAsync("desafio.dono@faculdade.edu.br");
        var id = await CreateAsync("Quem encerra", startOffset: -1, endOffset: 3, authorId: author.Id);

        using var scope = factory.Services.CreateScope();
        var challenges = scope.ServiceProvider.GetRequiredService<ChallengeService>();
        var other = await challenges.CloseAsync(new Editor("outro-professor", IsAdmin: false, IsTeacher: true), id);
        var owner = await challenges.CloseAsync(new Editor(author.Id, IsAdmin: false, IsTeacher: true), id);

        Assert.False(other.Succeeded);
        Assert.True(owner.Succeeded);
    }

    [Fact]
    public async Task Validation_RejectsBadDraftsAndLocksQuestionsAfterSubmissions()
    {
        var (author, editor) = await TeacherAsync("desafio.valida@faculdade.edu.br");
        using var scope = factory.Services.CreateScope();
        var challenges = scope.ServiceProvider.GetRequiredService<ChallengeService>();
        var today = challenges.Today();

        var backwards = await challenges.CreateAsync(editor, Draft("Datas", today, today.AddDays(-1), SeedQuestions));
        var discursive = await challenges.CreateAsync(editor, Draft("Discursiva", today, today, [8]));
        var empty = await challenges.CreateAsync(editor, Draft("Vazio", today, today, []));
        var duplicated = await challenges.CreateAsync(editor, Draft("Repetida", today, today, [1, 1]));

        Assert.Contains("O último dia não pode ser antes do primeiro.", backwards.Errors);
        Assert.Contains("Use só questões objetivas publicadas.", discursive.Errors);
        Assert.Contains("Escolha pelo menos uma questão.", empty.Errors);
        Assert.Contains("Cada questão só pode entrar uma vez.", duplicated.Errors);

        var id = await CreateAsync("Travado", startOffset: -1, endOffset: 1, authorId: author.Id);
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desafio.travou@gmail.com", Roles.Student);
        await challenges.SubmitAsync(student.Id, id, new Dictionary<int, char> { [1] = 'A', [2] = 'A', [5] = 'C' });

        var changeQuestions = await challenges.UpdateAsync(editor, id, Draft("Travado", today.AddDays(-1), today.AddDays(1), [1, 2]));
        var changeTitle = await challenges.UpdateAsync(editor, id, Draft("Título novo", today.AddDays(-1), today.AddDays(2), SeedQuestions));

        Assert.Contains("As questões não podem mudar depois que algum aluno enviou respostas.", changeQuestions.Errors);
        Assert.True(changeTitle.Succeeded);
    }

    [Fact]
    public async Task Form_IsForTeachersAndKeepsTheChosenQuestions()
    {
        var student = await StudentClientAsync("desafio.form.aluno@gmail.com");
        var studentResponse = await student.GetAsync("/desafios/novo");
        Assert.Contains("/Account/AccessDenied", studentResponse.Headers.Location?.ToString());

        var (teacher, _) = await TeacherAsync("desafio.form.prof@faculdade.edu.br");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, teacher.Email!);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var page = await client.GetStringAsync("/desafios/novo");

        var response = await TestAccounts.PostFormAsync(client, "/desafios/novo", page, "challenge-form", new List<KeyValuePair<string, string>>
        {
            new("Input.Title", "Desafio pelo formulário"),
            new("Input.Description", "Três questões"),
            new("Input.StartsOn", today.ToString("yyyy-MM-dd")),
            new("Input.EndsOn", today.AddDays(6).ToString("yyyy-MM-dd")),
            new("QuestionIds", "2"),
            new("QuestionIds", "5"),
        });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var id = int.Parse(response.Headers.Location!.ToString().Split('/')[^1]);
        using var scope = factory.Services.CreateScope();
        var challenge = await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Challenges
            .Include(c => c.Questions).SingleAsync(c => c.Id == id);
        Assert.Equal(teacher.Id, challenge.AuthorId);
        Assert.Equal([2, 5], challenge.Questions.OrderBy(q => q.Order).Select(q => q.QuestionId));
    }

    private async Task<int> CreateAsync(string title, int startOffset, int endOffset, string? authorId = null)
    {
        using var scope = factory.Services.CreateScope();
        var challenges = scope.ServiceProvider.GetRequiredService<ChallengeService>();
        var today = challenges.Today();
        var editor = new Editor(authorId ?? (await TeacherAsync($"autor.{Guid.NewGuid():N}@faculdade.edu.br")).User.Id, IsAdmin: false, IsTeacher: true);
        var result = await challenges.CreateAsync(editor, Draft(title, today.AddDays(startOffset), today.AddDays(endOffset), SeedQuestions));
        Assert.True(result.Succeeded, string.Join(" ", result.Errors));
        return result.Id!.Value;
    }

    private async Task CloseWithServiceAsync(int id)
    {
        using var scope = factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ChallengeService>().CloseAsync(new Editor("admin", IsAdmin: true, IsTeacher: false), id);
    }

    private static ChallengeDraft Draft(string title, DateOnly startsOn, DateOnly endsOn, IEnumerable<int> questionIds) => new()
    {
        Title = title,
        Description = "",
        StartsOn = startsOn,
        EndsOn = endsOn,
        QuestionIds = questionIds.ToList(),
    };

    private async Task<(ApplicationUser User, Editor Editor)> TeacherAsync(string email)
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student, Roles.Teacher);
        return (user, new Editor(user.Id, IsAdmin: false, IsTeacher: true));
    }

    private async Task<HttpClient> StudentClientAsync(string email)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }

    private static async Task<HttpResponseMessage> SubmitAsync(HttpClient client, int challengeId, Dictionary<int, char> answers, string? tokenPage = null)
    {
        var url = $"/desafios/{challengeId}";
        var page = await client.GetStringAsync(tokenPage ?? url);
        var fields = answers.ToDictionary(pair => $"q-{pair.Key}", pair => pair.Value.ToString());
        return await TestAccounts.PostFormAsync(client, url, page, "challenge-answers", fields);
    }

    private async Task<int> SubmissionCountAsync(int challengeId)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().ChallengeSubmissions.CountAsync(s => s.ChallengeId == challengeId);
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));
}
