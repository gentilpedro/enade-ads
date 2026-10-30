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
public class BlogTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory]
    [InlineData("Edital do ENADE 2026: o que muda?", "edital-do-enade-2026-o-que-muda")]
    [InlineData("  Ação & Reação — prazos!  ", "acao-reacao-prazos")]
    [InlineData("!!!", "post")]
    public void Slug_IsReadableAndUrlSafe(string title, string expected)
    {
        Assert.Equal(expected, Slugs.From(title));
    }

    [Fact]
    public async Task Slug_AvoidsReservedAndDuplicatesAndStaysAfterPublishing()
    {
        var (_, editor) = await TeacherAsync("blog.slug@faculdade.edu.br");
        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();

        var reserved = await blog.CreateAsync(editor, Draft("Novo"), PostAction.SaveDraft);
        var first = await blog.CreateAsync(editor, Draft("Calendário oficial"), PostAction.Publish);
        var second = await blog.CreateAsync(editor, Draft("Calendário oficial"), PostAction.Publish);
        await blog.UpdateAsync(editor, first.Id!.Value, Draft("Calendário oficial atualizado"), PostAction.Publish);

        Assert.Equal("novo-2", (await blog.FindAsync(reserved.Id!.Value))!.Slug);
        Assert.Equal("calendario-oficial", (await blog.FindAsync(first.Id.Value))!.Slug);
        Assert.Equal("calendario-oficial-2", (await blog.FindAsync(second.Id!.Value))!.Slug);
    }

    [Fact]
    public void Formatter_RendersSafeLinksAndLists()
    {
        var html = StatementFormatter.ToHtml("Veja o [edital](https://www.gov.br/inep/edital).\n\n- Inscrição\n- **Prova** em 29/11");

        Assert.Contains("<a href=\"https://www.gov.br/inep/edital\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">edital</a>", html);
        Assert.Contains("<ul><li>Inscrição</li><li><strong>Prova</strong> em 29/11</li></ul>", html);
    }

    [Theory]
    [InlineData("[clique](javascript:alert(1))")]
    [InlineData("[clique](http://exemplo.com)")]
    [InlineData("[<img src=x onerror=alert(1)>](https://exemplo.com)")]
    public void Formatter_NeverEmitsUnsafeLinks(string text)
    {
        var html = StatementFormatter.ToHtml(text);

        Assert.DoesNotContain("href=\"javascript", html);
        Assert.DoesNotContain("href=\"http://", html);
        Assert.DoesNotContain("<img src=x", html);
    }

    [Fact]
    public async Task Draft_IsVisibleOnlyToItsAuthor()
    {
        var (author, editor) = await TeacherAsync("blog.rascunho@faculdade.edu.br");
        var slug = await CreateAsync(editor, "Rascunho secreto do professor", PostAction.SaveDraft);
        var student = await StudentClientAsync("blog.curioso@gmail.com");
        var teacher = NewClient();
        await TestAccounts.LoginAsync(teacher, author.Email!);

        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync($"/blog/{slug}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await student.GetAsync($"/blog/{slug}")).StatusCode);
        Assert.DoesNotContain("Rascunho secreto do professor", await GetHtmlAsync(NewClient(), "/blog"));
        Assert.Contains("só você e os administradores veem", await GetHtmlAsync(teacher, $"/blog/{slug}"));
    }

    [Fact]
    public async Task Teacher_PublishesThroughTheForm()
    {
        var (author, _) = await TeacherAsync("blog.form@faculdade.edu.br");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, author.Email!);

        var response = await TestAccounts.SubmitFormAsync(client, "/blog/novo", "post-form", new()
        {
            ["Input.Title"] = "Local de prova do ENADE 2026 divulgado",
            ["Input.Summary"] = "Confira onde você vai fazer a prova.",
            ["Input.Body"] = "O cartão de confirmação já está no sistema.\n\n- Leve documento com foto\n- Caneta preta",
            ["Input.Category"] = nameof(PostCategory.DatesAndDeadlines),
            ["Input.SourceUrl"] = "https://www.gov.br/inep/pt-br",
            ["Command"] = "publicar",
        });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.EndsWith("/blog/local-de-prova-do-enade-2026-divulgado", response.Headers.Location!.ToString());
        var page = await GetHtmlAsync(NewClient(), "/blog/local-de-prova-do-enade-2026-divulgado");
        Assert.Contains("<li>Leve documento com foto</li>", page);
        Assert.Contains("Fonte:", page);
        Assert.Contains("Local de prova do ENADE 2026 divulgado", await GetHtmlAsync(NewClient(), "/blog?categoria=datas-e-prazos"));
        Assert.DoesNotContain("Local de prova do ENADE 2026 divulgado", await GetHtmlAsync(NewClient(), "/blog?categoria=dicas-de-estudo"));
    }

    [Fact]
    public async Task Students_CannotOpenTheFormAndSourceMustBeHttps()
    {
        var student = await StudentClientAsync("blog.aluno.form@gmail.com");
        var denied = await student.GetAsync("/blog/novo");
        var (_, editor) = await TeacherAsync("blog.fonte@faculdade.edu.br");

        using var scope = factory.Services.CreateScope();
        var draft = Draft("Fonte insegura");
        draft.SourceUrl = "javascript:alert(1)";
        var result = await scope.ServiceProvider.GetRequiredService<BlogService>().CreateAsync(editor, draft, PostAction.Publish);

        Assert.Contains("/Account/AccessDenied", denied.Headers.Location?.ToString());
        Assert.Contains(result.Errors, e => e.StartsWith("A fonte precisa ser um endereço https"));
    }

    [Fact]
    public async Task Comments_RequireLoginAndAreShownAsText()
    {
        var (_, editor) = await TeacherAsync("blog.comentarios@faculdade.edu.br");
        var slug = await CreateAsync(editor, "Post para comentar", PostAction.Publish);
        var student = await StudentClientAsync("blog.comenta@gmail.com");

        var visitor = await PostCommentAsync(NewClient(), slug, "Comentário de visitante", tokenPage: "/Account/Login");
        var response = await PostCommentAsync(student, slug, "<script>alert('x')</script> Quando sai o local de prova?");
        var raw = await response.Content.ReadAsStringAsync();

        Assert.False(visitor.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Quando sai o local de prova?", WebUtility.HtmlDecode(raw));
        Assert.DoesNotContain("<script>alert", raw);
        Assert.Equal(1, await CommentCountAsync(slug));
    }

    [Fact]
    public async Task OnlyAuthorEditsAndAuthorOrTeacherDeletes()
    {
        var (_, editor) = await TeacherAsync("blog.moderacao@faculdade.edu.br");
        var slug = await CreateAsync(editor, "Post moderado", PostAction.Publish);
        var author = await TestAccounts.CreateConfirmedAsync(factory, "blog.autor.comentario@gmail.com", Roles.Student);
        var other = await TestAccounts.CreateConfirmedAsync(factory, "blog.outro.aluno@gmail.com", Roles.Student);

        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var post = await blog.FindBySlugAsync(slug);
        var comment = await blog.AddCommentAsync(author.Id, post!.Id, "Texto original");
        var spam = await blog.AddCommentAsync(other.Id, post.Id, "Propaganda");

        var studentEditsOthers = await blog.EditCommentAsync(other.Id, comment!.Id, "Trocado");
        var authorEdits = await blog.EditCommentAsync(author.Id, comment.Id, "Texto revisado");
        var studentDeletesOthers = await blog.DeleteCommentAsync(null, other.Id, comment.Id);
        var teacherModerates = await blog.DeleteCommentAsync(editor, editor.UserId, spam!.Id);

        Assert.False(studentEditsOthers);
        Assert.True(authorEdits);
        Assert.False(studentDeletesOthers);
        Assert.True(teacherModerates);
        var remaining = Assert.Single(await blog.CommentsAsync(post.Id));
        Assert.Equal("Texto revisado", remaining.Comment.Text);
        Assert.NotNull(remaining.Comment.EditedAt);
    }

    [Fact]
    public async Task Pin_KeepsOnlyOnePostOnTopAndArchiveHidesIt()
    {
        var (_, editor) = await TeacherAsync("blog.fixar@faculdade.edu.br");
        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var first = (await blog.CreateAsync(editor, Draft("Primeiro fixado"), PostAction.Publish)).Id!.Value;
        var second = (await blog.CreateAsync(editor, Draft("Segundo fixado"), PostAction.Publish)).Id!.Value;
        var draft = (await blog.CreateAsync(editor, Draft("Rascunho não fixa"), PostAction.SaveDraft)).Id!.Value;

        await blog.SetPinnedAsync(editor, first, pinned: true);
        await blog.SetPinnedAsync(editor, second, pinned: true);
        var pinDraft = await blog.SetPinnedAsync(editor, draft, pinned: true);

        Assert.Null((await blog.FindAsync(first))!.PinnedAt);
        Assert.NotNull((await blog.FindAsync(second))!.PinnedAt);
        Assert.False(pinDraft.Succeeded);
        Assert.Equal("Segundo fixado", (await blog.ListAsync(null, 1)).Items[0].Title);
        Assert.Contains("Segundo fixado", await GetHtmlAsync(NewClient(), "/"));

        await blog.ArchiveAsync(editor, second);
        var archived = await blog.FindAsync(second);
        Assert.Null(archived!.PinnedAt);
        Assert.Equal(HttpStatusCode.NotFound, (await NewClient().GetAsync($"/blog/{archived.Slug}")).StatusCode);
        Assert.DoesNotContain("Segundo fixado", await GetHtmlAsync(NewClient(), "/blog"));
    }

    [Fact]
    public async Task Feed_IsValidRssWithOnlyPublishedPosts()
    {
        var (_, editor) = await TeacherAsync("blog.feed@faculdade.edu.br");
        await CreateAsync(editor, "Feed: <prova> & \"datas\"", PostAction.Publish);
        await CreateAsync(editor, "Feed: rascunho escondido", PostAction.SaveDraft);

        var response = await NewClient().GetAsync(BlogFeed.Path);
        var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());
        var titles = xml.Descendants("item").Select(i => i.Element("title")!.Value).ToList();

        Assert.Equal("application/rss+xml", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("2.0", xml.Root!.Attribute("version")!.Value);
        Assert.Contains("Feed: <prova> & \"datas\"", titles);
        Assert.DoesNotContain("Feed: rascunho escondido", titles);
    }

    private static PostDraft Draft(string title) => new()
    {
        Title = title,
        Summary = $"Resumo de {title}",
        Body = $"Texto de {title}",
        Category = PostCategory.ClassAnnouncements,
    };

    private async Task<string> CreateAsync(Editor editor, string title, PostAction action)
    {
        using var scope = factory.Services.CreateScope();
        var blog = scope.ServiceProvider.GetRequiredService<BlogService>();
        var result = await blog.CreateAsync(editor, Draft(title), action);
        Assert.True(result.Succeeded, string.Join(" ", result.Errors));
        return (await blog.FindAsync(result.Id!.Value))!.Slug;
    }

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

    private static async Task<HttpResponseMessage> PostCommentAsync(HttpClient client, string slug, string text, string? tokenPage = null)
    {
        var url = $"/blog/{slug}";
        var page = await client.GetStringAsync(tokenPage ?? url);
        return await TestAccounts.PostFormAsync(client, url, page, "comment-add", new() { ["CommentText"] = text });
    }

    private async Task<int> CommentCountAsync(string slug)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var post = await db.Posts.SingleAsync(p => p.Slug == slug);
        return await db.PostComments.CountAsync(c => c.PostId == post.Id);
    }

    private static async Task<string> GetHtmlAsync(HttpClient client, string url) =>
        WebUtility.HtmlDecode(await client.GetStringAsync(url));
}
