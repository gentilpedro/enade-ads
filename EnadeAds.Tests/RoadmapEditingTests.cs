using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class RoadmapEditingTests(EnadeAdsFactory factory)
{
    // Empreendedorismo não é usado por outros testes: as edições aqui não mudam o que eles conferem.
    private const string SubjectSlug = "empreendedorismo";

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task OnlyTeachersAndAdminsOpenTheEditor()
    {
        var visitor = await NewClient().GetAsync($"/roadmap/editar/{SubjectSlug}");
        var student = await ClientAsync("roadmap.edit.aluno@gmail.com", Roles.Student);
        var studentResponse = await student.GetAsync($"/roadmap/editar/{SubjectSlug}");
        var studentDrawer = await GetHtmlAsync(student, $"/roadmap?topico={SubjectSlug}");
        var teacher = await ClientAsync("roadmap.edit.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var teacherDrawer = await GetHtmlAsync(teacher, $"/roadmap?topico={SubjectSlug}");

        Assert.Contains("/Account/Login", visitor.Headers.Location?.ToString());
        Assert.Contains("/Account/AccessDenied", studentResponse.Headers.Location?.ToString());
        Assert.DoesNotContain("Editar este tópico", studentDrawer);
        Assert.Contains("Editar este tópico", teacherDrawer);
    }

    [Fact]
    public async Task Teacher_EditsSubjectThroughTheFormAndSlugStays()
    {
        var teacher = await ClientAsync("roadmap.edit.form@faculdade.edu.br", Roles.Student, Roles.Teacher);

        var response = await TestAccounts.SubmitFormAsync(teacher, $"/roadmap/editar/{SubjectSlug}", "node-form", new()
        {
            ["Input.Title"] = "Empreendedorismo",
            ["Input.Summary"] = "Resumo revisado pelo professor.",
        });

        // Professor não publica direto: a mudança vale depois que outro professor aprova.
        Assert.Contains("Enviado para revisão", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        Assert.NotEqual("Resumo revisado pelo professor.", (await NodeAsync(SubjectSlug)).Summary);
        int reviewId;
        using (var scope = factory.Services.CreateScope())
        {
            reviewId = await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().ContentReviews
                .Where(r => r.Kind == EnadeAds.Reviews.ReviewKind.RoadmapUpdate && r.Status == EnadeAds.Reviews.ReviewStatus.Pending)
                .OrderByDescending(r => r.Id).Select(r => r.Id).FirstAsync();
        }

        await TestReviews.ApproveAsync(factory, reviewId);
        var node = await NodeAsync(SubjectSlug);
        Assert.Equal("Resumo revisado pelo professor.", node.Summary);
        Assert.Contains("Resumo revisado pelo professor.", await GetHtmlAsync(NewClient(), $"/roadmap?topico={SubjectSlug}"));
    }

    [Fact]
    public async Task AddTopicAndCheckpoint_LandInTheRightPlace()
    {
        var editor = await EditorAsync("roadmap.edit.add@faculdade.edu.br");
        var subject = await NodeAsync(SubjectSlug);

        using var scope = factory.Services.CreateScope();
        var roadmap = scope.ServiceProvider.GetRequiredService<RoadmapEditor>();
        var topic = await roadmap.AddTopicAsync(editor, subject.Id, "Pitch e apresentação", "Como apresentar uma ideia em poucos minutos.");
        var checkpoint = await roadmap.AddCheckpointAfterAsync(editor, subject.Id, "Checkpoint: plano de negócio", "Monte um canvas.");

        var topicNode = await NodeAsync(topic.Id!.Value);
        var checkpointNode = await NodeAsync(checkpoint.Id!.Value);
        var spine = await SpineAsync();

        Assert.True(topicNode.Id >= 20_000);
        Assert.Equal("pitch-e-apresentacao", topicNode.Slug);
        Assert.Equal(subject.Id, topicNode.ParentId);
        Assert.Equal(await LastTopicOrderAsync(subject.Id), topicNode.Order);
        Assert.Equal(NodeKind.Checkpoint, checkpointNode.Kind);
        Assert.Equal(spine.IndexOf(subject.Slug) + 1, spine.IndexOf(checkpointNode.Slug));
        Assert.Equal(spine.Count, spine.Distinct().Count());
    }

    [Fact]
    public async Task Move_SwapsWithNeighborAndStopsAtTheEdges()
    {
        var editor = await EditorAsync("roadmap.edit.move@faculdade.edu.br");
        var subject = await NodeAsync(SubjectSlug);

        using var scope = factory.Services.CreateScope();
        var roadmap = scope.ServiceProvider.GetRequiredService<RoadmapEditor>();
        var first = (await roadmap.AddTopicAsync(editor, subject.Id, "Mover primeiro", "x")).Id!.Value;
        var second = (await roadmap.AddTopicAsync(editor, subject.Id, "Mover segundo", "x")).Id!.Value;
        var firstOrder = (await NodeAsync(first)).Order;

        await roadmap.MoveAsync(editor, second, up: true);
        var lastDown = await roadmap.MoveAsync(editor, first, up: false);

        Assert.Equal(firstOrder, (await NodeAsync(second)).Order);
        Assert.False(lastDown.Succeeded);
        Assert.Contains("Já é o último.", lastDown.Errors);
    }

    [Fact]
    public async Task SpineMovesStayInsideTheBlock()
    {
        var editor = await EditorAsync("roadmap.edit.bloco@faculdade.edu.br");

        using var scope = factory.Services.CreateScope();
        var roadmap = scope.ServiceProvider.GetRequiredService<RoadmapEditor>();

        // Lógica é a primeira matéria do primeiro bloco: não há para onde subir.
        var result = await roadmap.MoveAsync(editor, (await NodeAsync("logica-matematica")).Id, up: true);

        Assert.False(result.Succeeded);
        Assert.Contains("Já é o primeiro.", result.Errors);
    }

    [Fact]
    public async Task Remove_DeletesOnlyThatNodeProgressAndNeverASubject()
    {
        var editor = await EditorAsync("roadmap.edit.remove@faculdade.edu.br");
        var student = await TestAccounts.CreateConfirmedAsync(factory, "roadmap.edit.progresso@gmail.com", Roles.Student);
        var subject = await NodeAsync(SubjectSlug);

        using var scope = factory.Services.CreateScope();
        var roadmap = scope.ServiceProvider.GetRequiredService<RoadmapEditor>();
        var progress = scope.ServiceProvider.GetRequiredService<RoadmapService>();
        var temporary = await roadmap.AddTopicAsync(editor, subject.Id, "Assunto temporário", "Vai ser removido.");
        var slug = (await NodeAsync(temporary.Id!.Value)).Slug;
        await progress.SetStatusAsync(student.Id, slug, NodeStatus.Done);
        await progress.SetStatusAsync(student.Id, "mvp-startups", NodeStatus.Done);

        var removed = await roadmap.RemoveAsync(editor, temporary.Id.Value);
        var subjectRemoval = await roadmap.RemoveAsync(editor, subject.Id);
        var view = await progress.GetAsync(student.Id);

        Assert.True(removed.Succeeded);
        Assert.Null(view.Find(slug));
        Assert.Equal(NodeStatus.Done, view.Find("mvp-startups")!.Status);
        Assert.False(subjectRemoval.Succeeded);
        Assert.NotNull(view.Find(SubjectSlug));
    }

    [Fact]
    public async Task ExamDate_IsSetByAdminOnlyAndNotInThePast()
    {
        var teacher = await ClientAsync("roadmap.edit.data.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var admin = await ClientAsync("roadmap.edit.data.admin@faculdade.edu.br", Roles.Student, Roles.Admin);
        DateOnly today;
        using (var scope = factory.Services.CreateScope())
        {
            today = scope.ServiceProvider.GetRequiredService<LocalCalendar>().Today();
        }

        try
        {
            var denied = await teacher.GetAsync("/admin/data-da-prova");
            var past = await PostDateAsync(admin, today.AddDays(-1));
            var future = await PostDateAsync(admin, today.AddDays(40));
            var roadmap = await GetHtmlAsync(NewClient(), "/roadmap");

            Assert.Contains("/Account/AccessDenied", denied.Headers.Location?.ToString());
            Assert.Equal(HttpStatusCode.BadRequest, past.StatusCode);
            Assert.Equal(HttpStatusCode.OK, future.StatusCode);
            Assert.Contains(today.AddDays(40).ToString("dd/MM/yyyy"), roadmap);
        }
        finally
        {
            // Volta para a data usada pelos outros testes (contagem regressiva do roadmap).
            using var scope = factory.Services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ExamSettings>().SetExamDatesAsync(new DateOnly(2026, 11, 29));
        }
    }

    private static Task<HttpResponseMessage> PostDateAsync(HttpClient client, DateOnly date) =>
        TestAccounts.SubmitFormAsync(client, "/admin/data-da-prova", "exam-date", new() { ["Input.Date"] = date.ToString("yyyy-MM-dd") });

    private async Task<Editor> EditorAsync(string email)
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student, Roles.Teacher);
        return new Editor(user.Id, IsAdmin: false, IsTeacher: true, Reviewed: true);
    }

    private async Task<HttpClient> ClientAsync(string email, params string[] roles)
    {
        await TestAccounts.CreateConfirmedAsync(factory, email, roles);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, email);
        return client;
    }

    private async Task<RoadmapNode> NodeAsync(string slug)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().RoadmapNodes.AsNoTracking().SingleAsync(n => n.Slug == slug);
    }

    private async Task<RoadmapNode> NodeAsync(int id)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().RoadmapNodes.AsNoTracking().SingleAsync(n => n.Id == id);
    }

    private async Task<int> LastTopicOrderAsync(int subjectId)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().RoadmapNodes.Where(n => n.ParentId == subjectId).MaxAsync(n => n.Order);
    }

    private async Task<List<string>> SpineAsync()
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().RoadmapNodes
            .Where(n => n.ParentId == null).OrderBy(n => n.Order).Select(n => n.Slug).ToListAsync();
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));
}
