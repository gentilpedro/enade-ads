using System.Net;
using System.Xml.Linq;
using EnadeAds.Accounts;
using EnadeAds.Blog;
using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class CommunityFeedTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Student_PublishesADoubtThatIsNotAnOfficialNotice()
    {
        var (student, _) = await StudentAsync("feed.duvida@gmail.com");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, student.Email!);

        var response = await TestAccounts.SubmitFormAsync(client, "/blog/novo", "post-form", new()
        {
            ["Input.Title"] = "Pilha ou fila no exercício de escalonamento?",
            ["Input.Summary"] = "",
            ["Input.Body"] = "No simulado caiu uma questão de **escalonamento** e fiquei na dúvida entre pilha e fila.",
            ["Input.Category"] = nameof(PostCategory.Doubts),
            ["Command"] = "publicar",
        });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var post = await FindAsync("pilha-ou-fila-no-exercicio-de-escalonamento");
        Assert.False(post.IsOfficial);
        Assert.Equal("No simulado caiu uma questão de escalonamento e fiquei na dúvida entre pilha e fila.", post.Summary);

        Assert.Contains("Pilha ou fila no exercício de escalonamento?", await GetHtmlAsync(NewClient(), "/?ordem=novos"));
        Assert.DoesNotContain("Pilha ou fila no exercício de escalonamento?", await GetHtmlAsync(NewClient(), "/?ordem=oficiais"));
        var rss = XDocument.Parse(await NewClient().GetStringAsync(BlogFeed.Path));
        Assert.DoesNotContain("Pilha ou fila no exercício de escalonamento?", rss.Descendants("title").Select(t => t.Value));
    }

    [Fact]
    public async Task Student_CannotUseOfficialCategoriesAndTeacherPostIsOfficial()
    {
        var (_, student) = await StudentAsync("feed.categoria@gmail.com");
        var teacherUser = await TestAccounts.CreateConfirmedAsync(factory, "feed.professor@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var teacher = new Editor(teacherUser.Id, IsAdmin: false, IsTeacher: true);

        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var denied = await blog.CreateAsync(student, Draft("Prova adiada!", PostCategory.DatesAndDeadlines), PostAction.Publish);
        var official = await blog.CreateAsync(teacher, Draft("Aviso da coordenação", PostCategory.ClassAnnouncements), PostAction.Publish);

        Assert.False(denied.Succeeded);
        Assert.Contains(denied.Errors, e => e.Contains("é para avisos de professores"));
        Assert.True((await blog.FindAsync(official.Id!.Value))!.IsOfficial);
        Assert.Contains("Aviso da coordenação", await GetHtmlAsync(NewClient(), "/?ordem=oficiais"));
    }

    [Fact]
    public async Task Vote_TogglesAndSwitchesAndKeepsTheScore()
    {
        var (author, editor) = await StudentAsync("feed.votos.autor@gmail.com");
        var (voter, _) = await StudentAsync("feed.votos.um@gmail.com");
        var (other, _) = await StudentAsync("feed.votos.dois@gmail.com");
        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var postId = (await blog.CreateAsync(editor, Draft("Post para votar"), PostAction.Publish)).Id!.Value;
        var draftId = (await blog.CreateAsync(editor, Draft("Rascunho sem voto"), PostAction.SaveDraft)).Id!.Value;

        await blog.VoteAsync(voter.Id, postId, 1);
        await blog.VoteAsync(other.Id, postId, 1);
        Assert.Equal(2, await ScoreAsync(postId));

        await blog.VoteAsync(voter.Id, postId, -1);
        Assert.Equal(0, await ScoreAsync(postId));

        await blog.VoteAsync(voter.Id, postId, -1);
        Assert.Equal(1, await ScoreAsync(postId));
        Assert.Equal(0, await blog.MyVoteAsync(voter.Id, postId));

        Assert.False(await blog.VoteAsync(voter.Id, draftId, 1));
        Assert.False(await blog.VoteAsync(voter.Id, postId, 5));
        Assert.Equal(author.Id, (await blog.FindAsync(postId))!.AuthorId);
    }

    [Fact]
    public async Task VoteThroughTheFeed_RequiresLoginAndRedirectsBack()
    {
        var (_, editor) = await StudentAsync("feed.voto.http.autor@gmail.com");
        int postId;
        using (var scope = factory.Services.CreateScope())
        {
            postId = (await scope.ServiceProvider.GetRequiredService<BlogService>().CreateAsync(editor, Draft("Votado pela tela"), PostAction.Publish)).Id!.Value;
        }

        var visitor = NewClient();
        var anonymous = await TestAccounts.SubmitFormAsync(visitor, "/?ordem=novos", "feed-vote", new() { ["Vote"] = $"{postId}:1" });
        var (voter, _) = await StudentAsync("feed.voto.http@gmail.com");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, voter.Email!);
        var voted = await TestAccounts.SubmitFormAsync(client, "/?ordem=novos", "feed-vote", new() { ["Vote"] = $"{postId}:1" });

        Assert.Contains("Account/Login", anonymous.Headers.Location?.ToString());
        Assert.Equal(HttpStatusCode.Redirect, voted.StatusCode);
        Assert.EndsWith($"#post-{postId}", voted.Headers.Location!.ToString());
        Assert.Equal(1, await ScoreAsync(postId));
        Assert.Contains("aria-pressed=\"true\"", await client.GetStringAsync("/?ordem=novos"));
    }

    [Fact]
    public void Hotness_FavorsRecentPostsButLetsVotesWin()
    {
        var now = new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);

        Assert.True(BlogService.Hotness(0, now.AddHours(-1), now) > BlogService.Hotness(0, now.AddDays(-2), now));
        Assert.True(BlogService.Hotness(10, now.AddHours(-10), now) > BlogService.Hotness(0, now.AddHours(-1), now));
        Assert.True(BlogService.Hotness(-3, now, now) < BlogService.Hotness(0, now.AddDays(-30), now));
    }

    [Fact]
    public void Excerpt_DropsMarkupAndCutsLongText()
    {
        var excerpt = BlogService.Excerpt("Veja o **edital** em [Inep](https://www.gov.br/inep).\n\n![figura](https://x.com/a.png)\n- item `um`");
        var longText = BlogService.Excerpt(new string('a', 500));

        Assert.Equal("Veja o edital em Inep. item um", excerpt);
        Assert.Equal(BlogService.ExcerptLength + 1, longText.Length);
        Assert.EndsWith("…", longText);
    }

    [Fact]
    public async Task Students_ArePostRateLimited()
    {
        var (_, editor) = await StudentAsync("feed.spam@gmail.com");
        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();

        for (var i = 1; i <= BlogService.StudentPostsPerHour; i++)
        {
            Assert.True((await blog.CreateAsync(editor, Draft($"Spam {i}"), PostAction.Publish)).Succeeded);
        }

        var blocked = await blog.CreateAsync(editor, Draft("Spam demais"), PostAction.Publish);

        Assert.False(blocked.Succeeded);
        Assert.Contains("na última hora", blocked.Errors[0]);
    }

    [Fact]
    public async Task Reports_AreGroupedAndRemovalLocksThePostForItsAuthor()
    {
        var (author, editor) = await StudentAsync("feed.denuncia.autor@gmail.com");
        var (first, firstEditor) = await StudentAsync("feed.denuncia.um@gmail.com");
        var (second, _) = await StudentAsync("feed.denuncia.dois@gmail.com");
        var teacherUser = await TestAccounts.CreateConfirmedAsync(factory, "feed.moderador@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var teacher = new Editor(teacherUser.Id, IsAdmin: false, IsTeacher: true);

        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var postId = (await blog.CreateAsync(editor, Draft("Gabarito vazado da prova"), PostAction.Publish)).Id!.Value;

        Assert.True(await blog.ReportAsync(first.Id, postId, null, "Cola"));
        Assert.True(await blog.ReportAsync(first.Id, postId, null, "Cola de novo"));
        Assert.True(await blog.ReportAsync(second.Id, postId, null, null));
        var group = Assert.Single(await blog.OpenReportsAsync(), g => g.PostId == postId && g.CommentId is null);
        Assert.Equal(2, group.Count);
        Assert.Equal(new[] { "Cola" }, group.Reasons);

        Assert.False(await blog.RemoveAsync(firstEditor, postId));
        Assert.True(await blog.ResolveAsync(teacher, postId, null, removeContent: true));

        var removed = (await blog.FindAsync(postId))!;
        Assert.NotNull(removed.RemovedAt);
        Assert.DoesNotContain(await blog.OpenReportsAsync(), g => g.PostId == postId);
        Assert.False((await blog.UpdateAsync(editor, postId, Draft("Gabarito vazado da prova"), PostAction.Publish)).Succeeded);
        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync($"/blog/{removed.Slug}")).StatusCode);

        var authorClient = NewClient();
        await TestAccounts.LoginAsync(authorClient, author.Email!);
        Assert.Contains("Removido pela moderação", await GetHtmlAsync(authorClient, $"/blog/{removed.Slug}"));

        Assert.True(await blog.RestoreAsync(teacher, postId));
        Assert.True((await blog.FindAsync(postId))!.IsPublished);
    }

    [Fact]
    public async Task ReportedComment_CanBeRemovedOrKept()
    {
        var (_, editor) = await StudentAsync("feed.comentario.autor@gmail.com");
        var (reporter, _) = await StudentAsync("feed.comentario.denuncia@gmail.com");
        var teacherUser = await TestAccounts.CreateConfirmedAsync(factory, "feed.comentario.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var teacher = new Editor(teacherUser.Id, IsAdmin: false, IsTeacher: true);

        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var postId = (await blog.CreateAsync(editor, Draft("Post com comentários"), PostAction.Publish)).Id!.Value;
        var rude = await blog.AddCommentAsync(editor.UserId, postId, "Comentário ofensivo");
        var fine = await blog.AddCommentAsync(editor.UserId, postId, "Comentário normal");

        await blog.ReportAsync(reporter.Id, postId, rude!.Id, null);
        await blog.ReportAsync(reporter.Id, postId, fine!.Id, null);
        Assert.True(await blog.ResolveAsync(teacher, postId, rude.Id, removeContent: true));
        Assert.True(await blog.ResolveAsync(teacher, postId, fine.Id, removeContent: false));

        Assert.Equal(new[] { "Comentário normal" }, (await blog.CommentsAsync(postId)).Select(c => c.Comment.Text));
        Assert.DoesNotContain(await blog.OpenReportsAsync(), g => g.PostId == postId);
        Assert.False(await blog.ResolveAsync(new Editor(reporter.Id, false, false), postId, fine.Id, removeContent: true));
    }

    [Fact]
    public async Task ModerationPage_IsOnlyForTeachers()
    {
        var (student, _) = await StudentAsync("feed.moderacao.aluno@gmail.com");
        await TestAccounts.CreateConfirmedAsync(factory, "feed.moderacao.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var studentClient = NewClient();
        await TestAccounts.LoginAsync(studentClient, student.Email!);
        var teacherClient = NewClient();
        await TestAccounts.LoginAsync(teacherClient, "feed.moderacao.prof@faculdade.edu.br");

        var denied = await studentClient.GetAsync("/moderacao");
        var allowed = await teacherClient.GetAsync("/moderacao");

        Assert.Contains("/Account/AccessDenied", denied.Headers.Location?.ToString());
        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
    }

    private static PostDraft Draft(string title, PostCategory category = PostCategory.Discussion) => new()
    {
        Title = title,
        Body = $"Texto de {title}",
        Category = category,
    };

    private async Task<(ApplicationUser User, Editor Editor)> StudentAsync(string email)
    {
        var user = await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        return (user, new Editor(user.Id, IsAdmin: false, IsTeacher: false));
    }

    private async Task<Post> FindAsync(string slug)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Posts.AsNoTracking().SingleAsync(p => p.Slug == slug);
    }

    private async Task<int> ScoreAsync(int postId)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Posts.Where(p => p.Id == postId).Select(p => p.Score).SingleAsync();
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));
}
