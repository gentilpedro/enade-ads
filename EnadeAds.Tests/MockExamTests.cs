using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.MockExams;
using EnadeAds.Performance;
using EnadeAds.Questions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class MockExamTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory]
    [InlineData(46, 240)]
    [InlineData(23, 120)]
    [InlineData(11, 57)]
    [InlineData(1, 10)]
    public void Duration_IsProportionalToTheRealExam(int questions, int minutes)
    {
        Assert.Equal(minutes, MockExamFormat.MinutesFor(questions));
    }

    [Fact]
    public void Formats_FollowThePortariaProportion()
    {
        var full = MockExamFormat.Of(MockExamSize.Full);

        Assert.Equal((15, 30, 1), (full.GeneralEducation, full.Specific, full.Discursive));
        Assert.Equal(MockExamFormat.FullQuestionCount, full.Total);
        Assert.All(MockExamFormat.All, f => Assert.Equal(1, f.Discursive));
    }

    [Fact]
    public async Task Plans_FillMissingGeneralEducationWithSpecificQuestions()
    {
        using var scope = factory.Services.CreateScope();
        var plans = await scope.ServiceProvider.GetRequiredService<MockExamService>().PlansAsync();
        var shortPlan = plans.Single(p => p.Format.Size == MockExamSize.Short);

        // O banco de teste não tem formação geral: as 10 vagas objetivas vão para específicas
        // (a seed tem 7; outros testes podem ter criado mais).
        Assert.Equal(0, shortPlan.GeneralEducation);
        Assert.InRange(shortPlan.Specific, 7, 10);
        Assert.Equal(1, shortPlan.Discursive);
        Assert.All(plans, p => Assert.InRange(p.Total, 1, p.Format.Total));
    }

    [Fact]
    public async Task Visitor_IsInvitedAndCannotOpenAnExam()
    {
        var html = WebUtility.HtmlDecode(await NewClient().GetStringAsync("/simulados"));
        var exam = await NewClient().GetAsync("/simulados/1");

        Assert.Contains("para fazer um simulado", html);
        Assert.Equal(HttpStatusCode.Redirect, exam.StatusCode);
        Assert.Contains("/Account/Login", exam.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Start_SetsServerDeadlineAndAllowsOnlyOneOpenExam()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "simulado.inicio@gmail.com", Roles.Student);
        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MockExamService>();

        var first = await service.StartAsync(student.Id, MockExamSize.Short);
        var second = await service.StartAsync(student.Id, MockExamSize.Full);
        var exam = await service.FindAsync(student.Id, first.MockExamId!.Value);

        Assert.Equal(StartOutcome.Started, first.Outcome);
        Assert.Equal(StartOutcome.AlreadyOpen, second.Outcome);
        Assert.Equal(first.MockExamId, second.MockExamId);
        Assert.Equal(MockExamFormat.MinutesFor(exam!.Questions.Count), (int)(exam.DeadlineAt - exam.StartedAt).TotalMinutes);
        Assert.Equal(QuestionKind.Discursive, exam.Questions[^1].Question!.Kind);
        Assert.Equal(exam.Questions.Count - 1, exam.ObjectiveCount);
    }

    [Fact]
    public async Task Exam_IsPrivateToItsOwner()
    {
        var owner = await TestAccounts.CreateConfirmedAsync(factory, "simulado.dono@gmail.com", Roles.Student);
        var id = await StartAsync(owner.Id);
        var other = await StudentClientAsync("simulado.intruso@gmail.com");

        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/simulados/{id}")).StatusCode);
    }

    [Fact]
    public async Task SaveKeepsAnswersWithoutRevealingTheAnswerKey()
    {
        var (client, user) = await StudentWithClientAsync("simulado.salva@gmail.com");
        var id = await StartAsync(user.Id);
        var exam = await FindAsync(user.Id, id);
        var objective = exam.Questions.First(q => q.Question!.Kind == QuestionKind.Objective);
        var discursive = exam.Questions.Single(q => q.Question!.Kind == QuestionKind.Discursive);

        var response = await PostAsync(client, id, new()
        {
            [$"q-{objective.QuestionId}"] = "B",
            [$"t-{discursive.QuestionId}"] = "Rascunho da discursiva",
            ["Command"] = "salvar",
        });
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        var saved = await FindAsync(user.Id, id);

        Assert.Contains("Respostas salvas.", html);
        Assert.Contains("Rascunho da discursiva", html);
        Assert.DoesNotContain("badge text-bg-success ms-1\">Correta", html);
        Assert.True(saved.IsOpen);
        Assert.NotNull(saved.Questions.Single(q => q.QuestionId == objective.QuestionId).SelectedOptionId);
    }

    [Fact]
    public async Task SubmitNeedsConfirmationThenShowsResultAndFeedsThePanel()
    {
        var (client, user) = await StudentWithClientAsync("simulado.entrega@gmail.com");
        var id = await StartAsync(user.Id);
        var exam = await FindAsync(user.Id, id);
        var answers = exam.Questions.Where(q => q.Question!.Kind == QuestionKind.Objective)
            .ToDictionary(q => $"q-{q.QuestionId}", q => q.Question!.Options.Single(o => o.IsCorrect).Letter.ToString());

        var unconfirmed = await PostAsync(client, id, new(answers) { ["Command"] = "entregar" });
        var confirmed = await PostAsync(client, id, new(answers) { ["Command"] = "entregar", ["ConfirmSubmit"] = "true" });
        var html = WebUtility.HtmlDecode(await confirmed.Content.ReadAsStringAsync());

        Assert.Contains("marque \"Terminei e quero entregar\"", WebUtility.HtmlDecode(await unconfirmed.Content.ReadAsStringAsync()));
        Assert.Contains($"Você acertou {exam.ObjectiveCount} de {exam.ObjectiveCount}", html);
        Assert.Contains("Padrão de resposta", html);
        Assert.False((await FindAsync(user.Id, id)).IsOpen);

        using var scope = factory.Services.CreateScope();
        var report = await scope.ServiceProvider.GetRequiredService<PerformanceService>().GetAsync(user.Id);
        Assert.Equal(new Score(exam.ObjectiveCount, exam.ObjectiveCount), report.Overall);
    }

    [Fact]
    public async Task WhenTimeRunsOut_ExamIsSubmittedWithSavedAnswersOnly()
    {
        var (client, user) = await StudentWithClientAsync("simulado.tempo@gmail.com");
        var id = await StartAsync(user.Id);
        var exam = await FindAsync(user.Id, id);
        var question = exam.Questions.First(q => q.Question!.Kind == QuestionKind.Objective).Question!;
        var correct = question.Options.Single(o => o.IsCorrect).Letter;
        var wrong = question.Options.First(o => !o.IsCorrect).Letter;

        await PostAsync(client, id, new() { [$"q-{question.Id}"] = wrong.ToString(), ["Command"] = "salvar" });
        await MoveDeadlineAsync(id, TimeSpan.FromMinutes(-5));

        // Resposta enviada depois do prazo é ignorada: vale a que estava salva.
        using (var scope = factory.Services.CreateScope())
        {
            var outcome = await scope.ServiceProvider.GetRequiredService<MockExamService>()
                .SaveAsync(user.Id, id, new Dictionary<int, char> { [question.Id] = correct }, new Dictionary<int, string>(), submit: false);
            Assert.Equal(SaveOutcome.Submitted, outcome);
        }

        var finished = await FindAsync(user.Id, id);
        var html = WebUtility.HtmlDecode(await client.GetStringAsync($"/simulados/{id}"));

        Assert.True(finished.SubmittedByTimeout);
        Assert.Equal(0, finished.CorrectCount);
        Assert.Contains("O tempo acabou", html);
    }

    [Fact]
    public async Task ExpiredExam_IsClosedOnNextVisitSoANewOneCanStart()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "simulado.novo@gmail.com", Roles.Student);
        var id = await StartAsync(student.Id);
        await MoveDeadlineAsync(id, TimeSpan.FromHours(-1));

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MockExamService>();
        var next = await service.StartAsync(student.Id, MockExamSize.Short);

        Assert.Equal(StartOutcome.Started, next.Outcome);
        Assert.NotEqual(id, next.MockExamId);
        Assert.Single(await service.HistoryAsync(student.Id));
    }

    private async Task<int> StartAsync(string userId)
    {
        using var scope = factory.Services.CreateScope();
        var result = await scope.ServiceProvider.GetRequiredService<MockExamService>().StartAsync(userId, MockExamSize.Short);
        return result.MockExamId!.Value;
    }

    private async Task<MockExam> FindAsync(string userId, int id)
    {
        using var scope = factory.Services.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<MockExamService>().FindAsync(userId, id))!;
    }

    private async Task MoveDeadlineAsync(int id, TimeSpan fromNow)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exam = await db.MockExams.SingleAsync(m => m.Id == id);
        exam.DeadlineAt = DateTimeOffset.UtcNow + fromNow;
        await db.SaveChangesAsync();
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, int id, Dictionary<string, string> fields)
    {
        var url = $"/simulados/{id}";
        return await TestAccounts.PostFormAsync(client, url, await client.GetStringAsync(url), "mock-exam", fields);
    }

    private async Task<(HttpClient Client, ApplicationUser User)> StudentWithClientAsync(string email)
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return (client, user);
    }

    private async Task<HttpClient> StudentClientAsync(string email) => (await StudentWithClientAsync(email)).Client;
}
