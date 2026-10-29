using System.Net;
using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.Study;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class RoadmapPageTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Visitor_SeesRoadmapWithoutStatusButtons()
    {
        var html = await GetHtmlAsync(NewClient(), "/roadmap");

        Assert.Contains("Estruturas de dados", html);
        Assert.Contains("Checkpoint final: simulado completo", html);
        Assert.Contains("acompanhar seu progresso", html);
        Assert.DoesNotContain("node-status", html);
    }

    [Fact]
    public async Task Visitor_OpensTopicPanelWithLoginInvite()
    {
        var html = await GetHtmlAsync(NewClient(), "/roadmap?topico=pilhas");

        Assert.Contains("rm-drawer", html);
        Assert.Contains("LIFO", html);
        Assert.Contains("Estruturas de dados", html);
        Assert.Contains("para marcar este tópico", html);
    }

    [Fact]
    public async Task UnknownTopic_ShowsRoadmapWithoutPanel()
    {
        var html = await GetHtmlAsync(NewClient(), "/roadmap?topico=nao-existe");

        Assert.DoesNotContain("rm-drawer", html);
    }

    [Fact]
    public async Task Visitor_CannotPostStatus()
    {
        // O visitante não recebe o formulário; o token vem da página de login para simular um POST forjado.
        // O Blazor recusa (400) porque o formulário nem existe para visitante; o handler ainda confere o login (401).
        var response = await PostStatusAsync(NewClient(), "pilhas", "Done", tokenSource: "/Account/Login");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(0, await ProgressCountAsync("pilhas"));
    }

    [Fact]
    public async Task Student_MarksAndClearsTopic()
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, "roadmap.aluno@gmail.com", Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, "roadmap.aluno@gmail.com");

        var done = await PostStatusAsync(client, "filas", "Done");
        Assert.Equal(HttpStatusCode.OK, done.StatusCode);
        Assert.Equal(NodeStatus.Done, await StatusOfAsync(user.Id, "filas"));
        Assert.Contains("1 de", WebUtility.HtmlDecode(await done.Content.ReadAsStringAsync()));

        await PostStatusAsync(client, "filas", "InProgress");
        Assert.Equal(NodeStatus.InProgress, await StatusOfAsync(user.Id, "filas"));

        await PostStatusAsync(client, "filas", "Limpar");
        Assert.Null(await StatusOfAsync(user.Id, "filas"));
    }

    [Fact]
    public async Task InvalidStatus_IsRejected()
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, "roadmap.invalido@gmail.com", Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, "roadmap.invalido@gmail.com");

        var response = await PostStatusAsync(client, "grafos", "99");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(await StatusOfAsync(user.Id, "grafos"));
    }

    [Fact]
    public async Task Progress_IsPerStudent()
    {
        var first = await TestAccounts.CreateConfirmedAsync(factory, "roadmap.primeiro@gmail.com", Roles.Student);
        var second = await TestAccounts.CreateConfirmedAsync(factory, "roadmap.segundo@gmail.com", Roles.Student);

        using var scope = factory.Services.CreateScope();
        var roadmap = scope.ServiceProvider.GetRequiredService<RoadmapService>();
        await roadmap.SetStatusAsync(first.Id, "sql-join-group", NodeStatus.Done);

        var mine = await roadmap.GetAsync(first.Id);
        var theirs = await roadmap.GetAsync(second.Id);

        Assert.Equal(NodeStatus.Done, mine.Find("sql-join-group")!.Status);
        Assert.Null(theirs.Find("sql-join-group")!.Status);
        Assert.Equal(1, mine.Counts.Done);
        Assert.Equal(0, theirs.Counts.Done);
    }

    [Fact]
    public async Task Countdown_UsesExamDateInBrazilTime()
    {
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var exam = services.GetRequiredService<IOptions<ExamOptions>>();

        // 30/10/2026 01:00 UTC ainda é 29/10 em São Paulo: faltam 31 dias para 29/11.
        var clock = new FixedClock(new DateTimeOffset(2026, 10, 30, 1, 0, 0, TimeSpan.Zero));
        var roadmap = new RoadmapService(services.GetRequiredService<ApplicationDbContext>(), exam, clock);

        var view = await roadmap.GetAsync(null);

        Assert.Equal(31, view.DaysUntilExam);
    }

    [Fact]
    public async Task SubjectPages_AreOpenToVisitors()
    {
        var client = NewClient();

        Assert.Contains("Banco de dados", await GetHtmlAsync(client, "/materias"));
        Assert.Contains("Normalização", await GetHtmlAsync(client, "/materias/banco-de-dados"));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/materias/nao-existe")).StatusCode);
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));

    private static async Task<HttpResponseMessage> PostStatusAsync(HttpClient client, string slug, string command, string? tokenSource = null)
    {
        var url = $"/roadmap?topico={slug}";
        var tokenPage = tokenSource ?? url;
        var page = await client.GetStringAsync(tokenPage);
        return await TestAccounts.PostFormAsync(client, url, page, "node-status", new() { ["StatusCommand"] = command });
    }

    private async Task<NodeStatus?> StatusOfAsync(string userId, string slug)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var node = await db.RoadmapNodes.SingleAsync(n => n.Slug == slug);
        var progress = await db.RoadmapProgress.FindAsync(userId, node.Id);
        return progress?.Status;
    }

    private async Task<int> ProgressCountAsync(string slug)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var node = await db.RoadmapNodes.SingleAsync(n => n.Slug == slug);
        return await db.RoadmapProgress.CountAsync(p => p.NodeId == node.Id);
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
