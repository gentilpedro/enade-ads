using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Challenges;
using EnadeAds.Performance;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class PerformanceTests(EnadeAdsFactory factory)
{
    // Seed: 1 = estruturas de dados (A), 2 = arquitetura (A), 3 = redes (A), 4 = banco de dados (A), 6 = qualidade (A), 8 = discursiva.
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Page_RequiresLogin()
    {
        var response = await NewClient().GetAsync("/meu-desempenho");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task NewStudent_SeesEmptyStateWithoutErrors()
    {
        var client = await StudentClientAsync("desempenho.novo@gmail.com");

        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/meu-desempenho"));

        Assert.Contains("Você ainda não respondeu nenhuma questão objetiva", html);
        Assert.Contains("Nenhuma questão errada para refazer", html);
    }

    [Fact]
    public async Task LatestAnswerWins_AndMistakesListOnlyCurrentErrors()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.ultima@gmail.com", Roles.Student);
        await AnswerAsync(student.Id, (1, 'B'), (1, 'A'), (2, 'C'), (3, 'A'), (4, 'D'));

        var report = await ReportAsync(student.Id);

        Assert.Equal(new Score(4, 2), report.Overall);
        Assert.Equal(50, report.Overall.Percent);
        Assert.Equal([2, 4], report.Mistakes.Select(m => m.QuestionId));
        Assert.Equal(100, SubjectOf(report, "estruturas-dados").Score.Percent);
        Assert.Equal(new Score(1, 0), SubjectOf(report, "arquitetura-computadores").Score);
        Assert.Equal(new Score(0, 0), SubjectOf(report, "logica-matematica").Score);
    }

    [Fact]
    public async Task Weakest_ListsLowestAccuracyAmongAnsweredSubjects()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.fracas@gmail.com", Roles.Student);
        await AnswerAsync(student.Id, (1, 'A'), (2, 'B'), (3, 'A'), (4, 'C'), (6, 'D'));

        var report = await ReportAsync(student.Id);

        Assert.Equal(["arquitetura-computadores", "banco-de-dados", "qualidade-testes"], report.Weakest.Select(s => s.Slug).Order());
        Assert.DoesNotContain(report.Weakest, s => s.Slug == "estruturas-dados");
    }

    [Fact]
    public async Task OpenChallengeAnswersStayHiddenUntilItEnds()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.desafio@gmail.com", Roles.Student);
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var editor = new Editor(teacher.Id, IsAdmin: false, IsTeacher: true);

        int challengeId;
        using (var scope = factory.Services.CreateScope())
        {
            var challenges = scope.ServiceProvider.GetRequiredService<ChallengeService>();
            var today = challenges.Today();
            challengeId = (await challenges.CreateAsync(editor, new ChallengeDraft
            {
                Title = "Desafio do painel", StartsOn = today, EndsOn = today.AddDays(3), QuestionIds = [1, 2],
            })).Id!.Value;
            await challenges.SubmitAsync(student.Id, challengeId, new Dictionary<int, char> { [1] = 'B', [2] = 'A' });
        }

        var whileOpen = await ReportAsync(student.Id);

        using (var scope = factory.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ChallengeService>().CloseAsync(editor, challengeId);
        }

        var afterEnd = await ReportAsync(student.Id);

        Assert.Equal(new Score(0, 0), whileOpen.Overall);
        Assert.Empty(whileOpen.Mistakes);
        Assert.Equal(1, Assert.Single(whileOpen.Challenges).Correct);
        Assert.Equal(new Score(2, 1), afterEnd.Overall);
        Assert.Equal([1], afterEnd.Mistakes.Select(m => m.QuestionId));
    }

    [Fact]
    public async Task ReportIsPerStudentAndCountsDiscursivesAndRoadmap()
    {
        var first = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.primeiro@gmail.com", Roles.Student);
        var second = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.segundo@gmail.com", Roles.Student);
        await AnswerAsync(first.Id, (1, 'D'));

        using (var scope = factory.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<QuestionBank>().AnswerDiscursiveAsync(first.Id, 8, "Minha resposta");
            await scope.ServiceProvider.GetRequiredService<RoadmapService>().SetStatusAsync(first.Id, "pilhas", NodeStatus.Done);
        }

        var mine = await ReportAsync(first.Id);
        var theirs = await ReportAsync(second.Id);

        Assert.Equal(1, mine.DiscursivesAnswered);
        Assert.Equal(1, mine.Roadmap.Done);
        Assert.Single(mine.Mistakes);
        Assert.Equal(new Score(0, 0), theirs.Overall);
        Assert.Empty(theirs.Mistakes);
        Assert.Equal(0, theirs.DiscursivesAnswered);
    }

    [Fact]
    public async Task Page_ShowsMistakeWithRetryLink()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "desempenho.pagina@gmail.com", Roles.Student);
        await AnswerAsync(student.Id, (3, 'B'));
        var client = NewClient();
        await TestAccounts.LoginAsync(client, student.Email!);

        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/meu-desempenho"));

        Assert.Contains("Revise primeiro", html);
        Assert.Contains("questoes/3?refazer=1", html);
        Assert.Contains("Princípios de redes de computadores", html);
    }

    private async Task AnswerAsync(string userId, params (int QuestionId, char Letter)[] answers)
    {
        using var scope = factory.Services.CreateScope();
        var bank = scope.ServiceProvider.GetRequiredService<QuestionBank>();
        foreach (var (questionId, letter) in answers)
        {
            Assert.NotNull(await bank.AnswerAsync(userId, questionId, letter));
        }
    }

    private async Task<PerformanceReport> ReportAsync(string userId)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<PerformanceService>().GetAsync(userId);
    }

    private static SubjectScore SubjectOf(PerformanceReport report, string slug) => report.Subjects.Single(s => s.Slug == slug);

    private async Task<HttpClient> StudentClientAsync(string email)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }
}
