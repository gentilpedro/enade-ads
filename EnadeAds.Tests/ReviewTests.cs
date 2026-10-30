using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Reviews;
using EnadeAds.Study;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

/// <summary>Aprova, como outro professor, a proposta que uma página mandou para a revisão.</summary>
public static class TestReviews
{
    private const string ReviewerEmail = "revisor.testes@faculdade.edu.br";

    /// <summary>Lê o id da proposta no redirecionamento (<c>revisoes?enviada=N</c>), aprova e devolve o id do item.</summary>
    public static async Task<int?> ApproveFromRedirectAsync(EnadeAdsFactory factory, HttpResponseMessage response)
    {
        var location = response.Headers.Location?.ToString() ?? "";
        Assert.Contains("revisoes?enviada=", location);
        return await ApproveAsync(factory, int.Parse(location.Split('=')[^1]));
    }

    public static async Task<int?> ApproveAsync(EnadeAdsFactory factory, int reviewId)
    {
        var reviewer = await ReviewerAsync(factory);
        using var scope = factory.Services.CreateScope();
        var decisions = scope.ServiceProvider.GetRequiredService<ReviewDecisions>();
        var result = await decisions.ApproveAsync(new Editor(reviewer.Id, IsAdmin: false, IsTeacher: true), reviewId);
        Assert.True(result.Succeeded, result.Message);
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return (await db.ContentReviews.SingleAsync(r => r.Id == reviewId)).ResultId;
    }

    private static async Task<ApplicationUser> ReviewerAsync(EnadeAdsFactory factory)
    {
        using (var scope = factory.Services.CreateScope())
        {
            var existing = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByEmailAsync(ReviewerEmail);
            if (existing is not null)
            {
                return existing;
            }
        }

        return await TestAccounts.CreateConfirmedAsync(factory, ReviewerEmail, Roles.Student, Roles.Teacher);
    }
}

[Collection(AppCollection.Name)]
public class ReviewTests(EnadeAdsFactory factory)
{
    private const int SubjectId = 400;

    private static QuestionDraft Draft(string statement) => new()
    {
        Kind = QuestionKind.Objective,
        SubjectId = SubjectId,
        Statement = statement,
        Source = "Autoria própria",
        CorrectLetter = 'B',
        Options = Enumerable.Range(0, QuestionDraft.OptionCount)
            .Select(i => new OptionDraft { Text = $"Alternativa {i}", Explanation = $"Explicação {i}" }).ToList(),
    };

    private async Task<Editor> TeacherAsync(string email) =>
        new((await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student, Roles.Teacher)).Id, IsAdmin: false, IsTeacher: true);

    [Fact]
    public async Task TeacherEdit_WaitsForAnotherTeacherAndIsAppliedOnApproval()
    {
        var author = await TeacherAsync("revisao.autor@faculdade.edu.br");
        var colleague = await TeacherAsync("revisao.colega@faculdade.edu.br");

        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var queued = await services.GetRequiredService<QuestionEditor>().CreateAsync(author, Draft("Questão que espera revisão"));
        var db = services.GetRequiredService<ApplicationDbContext>();
        var decisions = services.GetRequiredService<ReviewDecisions>();

        Assert.True(queued.IsQueued);
        Assert.False(await db.Questions.AnyAsync(q => q.Statement == "Questão que espera revisão"));

        var self = await decisions.ApproveAsync(author, queued.QueuedReviewId!.Value);
        var approved = await decisions.ApproveAsync(colleague, queued.QueuedReviewId.Value);
        var review = await db.ContentReviews.AsNoTracking().SingleAsync(r => r.Id == queued.QueuedReviewId);
        var question = await db.Questions.AsNoTracking().SingleAsync(q => q.Id == review.ResultId);

        Assert.False(self.Succeeded);
        Assert.True(approved.Succeeded);
        Assert.Equal(ReviewStatus.Approved, review.Status);
        Assert.Equal(colleague.UserId, review.ReviewerId);
        // A questão é do autor da proposta, não de quem aprovou.
        Assert.Equal(author.UserId, question.AuthorId);
        Assert.Equal("Questão que espera revisão", question.Statement);
    }

    [Fact]
    public async Task AdminEdit_IsPublishedDirectly()
    {
        using var scope = factory.Services.CreateScope();
        var result = await scope.ServiceProvider.GetRequiredService<RoadmapEditor>()
            .AddTopicAsync(new Editor("admin-direto", IsAdmin: true, IsTeacher: false), SubjectId, "Assunto do admin", "Publicado sem fila.");

        Assert.False(result.IsQueued);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Rejection_KeepsContentAndTellsTheAuthorWhy()
    {
        var author = await TeacherAsync("revisao.recusa@faculdade.edu.br");
        var colleague = await TeacherAsync("revisao.recusa.colega@faculdade.edu.br");

        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var queued = await services.GetRequiredService<RoadmapEditor>().UpdateAsync(author, SubjectId, "Título trocado", "Resumo trocado");
        var decisions = services.GetRequiredService<ReviewDecisions>();

        var rejected = await decisions.RejectAsync(colleague, queued.QueuedReviewId!.Value, "Título fora da matriz da prova.");
        var mine = await decisions.MineAsync(author.UserId);
        var node = await services.GetRequiredService<ApplicationDbContext>().RoadmapNodes.AsNoTracking().SingleAsync(n => n.Id == SubjectId);

        Assert.True(rejected.Succeeded);
        Assert.NotEqual("Título trocado", node.Title);
        var item = Assert.Single(mine, i => i.Review.Id == queued.QueuedReviewId);
        Assert.Equal(ReviewStatus.Rejected, item.Review.Status);
        Assert.Equal("Título fora da matriz da prova.", item.Review.Note);
    }

    [Fact]
    public async Task Approval_RunsInTheTrackOfTheProposal()
    {
        var author = await TeacherAsync("revisao.enem@faculdade.edu.br");
        var colleague = await TeacherAsync("revisao.enem.colega@faculdade.edu.br");
        int reviewId;

        using (var enem = factory.Services.CreateScope())
        {
            enem.ServiceProvider.GetRequiredService<TrackScope>().Use(Tracks.Enem);
            var draft = Draft("Questão de Matemática do ENEM proposta");
            draft.SubjectId = EnemRoadmapSeed.Nodes.First(n => n.Slug == "matematica-basica").Id;
            reviewId = (await enem.ServiceProvider.GetRequiredService<QuestionEditor>().CreateAsync(author, draft)).QueuedReviewId!.Value;
        }

        // Quem aprova está na trilha padrão (ENADE); a questão precisa ser validada e criada na trilha do ENEM.
        using var scope = factory.Services.CreateScope();
        var result = await scope.ServiceProvider.GetRequiredService<ReviewDecisions>().ApproveAsync(colleague, reviewId);

        Assert.True(result.Succeeded, result.Message);
    }

    [Fact]
    public async Task ReviewPage_IsForEditorsOnly()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "revisao.aluno@gmail.com", Roles.Student);
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        await TestAccounts.LoginAsync(client, student.Email!);

        var response = await client.GetAsync("/revisoes");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/AccessDenied", response.Headers.Location?.ToString());
    }
}
