using System.Net;
using System.Text.Json;
using EnadeAds.Accounts;
using EnadeAds.Blog;
using EnadeAds.Data;
using EnadeAds.MockExams;
using EnadeAds.Privacy;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class PrivacyTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory]
    [InlineData("/privacidade", "Política de privacidade")]
    [InlineData("/termos", "Termos de uso")]
    public async Task LegalPagesAreOpenAndLinkedInTheFooter(string url, string title)
    {
        var page = WebUtility.HtmlDecode(await NewClient().GetStringAsync(url));
        var home = await NewClient().GetStringAsync("/");

        Assert.Contains(title, page);
        Assert.Contains($"href=\"{url.TrimStart('/')}\"", home);
    }

    [Fact]
    public async Task Register_RequiresAcceptingTheTermsAndStoresTheVersion()
    {
        var client = NewClient();
        var fields = new Dictionary<string, string>
        {
            ["Input.FullName"] = "Pessoa Termos",
            ["Input.Email"] = "privacidade.termos@gmail.com",
            ["Input.Password"] = TestAccounts.Password,
            ["Input.ConfirmPassword"] = TestAccounts.Password,
        };

        var refused = await TestAccounts.SubmitFormAsync(client, "/Account/Register", "register", fields);
        fields["Input.AcceptTerms"] = "true";
        var noAge = await TestAccounts.SubmitFormAsync(client, "/Account/Register", "register", fields);
        fields["Input.AgeGroup"] = "18+";
        var accepted = await TestAccounts.SubmitFormAsync(client, "/Account/Register", "register", fields);

        Assert.Contains("aceite os Termos de uso", WebUtility.HtmlDecode(await refused.Content.ReadAsStringAsync()));
        Assert.Contains("Menores de 13 anos não podem criar conta", WebUtility.HtmlDecode(await noAge.Content.ReadAsStringAsync()));
        Assert.Equal(HttpStatusCode.Redirect, accepted.StatusCode);
        using var scope = factory.Services.CreateScope();
        var user = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByEmailAsync("privacidade.termos@gmail.com");
        Assert.NotNull(user!.TermsAcceptedAt);
        Assert.Equal(PrivacyOptions.TermsVersion, user.TermsVersion);
        Assert.False(user.IsMinor);
        Assert.Null(user.GuardianConsentAt);
    }

    [Fact]
    public async Task Register_MinorDeclaresTheGuardianConsent()
    {
        var response = await TestAccounts.SubmitFormAsync(NewClient(), "/Account/Register", "register", new()
        {
            ["Input.FullName"] = "Estudante Menor",
            ["Input.Email"] = "privacidade.menor@gmail.com",
            ["Input.Password"] = TestAccounts.Password,
            ["Input.ConfirmPassword"] = TestAccounts.Password,
            ["Input.AcceptTerms"] = "true",
            ["Input.AgeGroup"] = "13-17",
        });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var user = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByEmailAsync("privacidade.menor@gmail.com");
        Assert.True(user!.IsMinor);
        Assert.Equal(user.TermsAcceptedAt, user.GuardianConsentAt);
    }

    [Fact]
    public async Task DownloadIncludesStudyDataAndNoAuthenticatorKey()
    {
        var student = await StudentWithActivityAsync("privacidade.download@gmail.com");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, student.Email!);

        var page = await client.GetStringAsync("/Account/Manage/PersonalData");
        var download = await TestAccounts.PostFormAsync(client, "/Account/Manage/DownloadPersonalData", page, "", []);
        var json = JsonDocument.Parse(await download.Content.ReadAsStringAsync()).RootElement;

        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal(student.Email, json.GetProperty("Conta").GetProperty("Email").GetString());
        Assert.Equal(1, json.GetProperty("Roadmap").GetArrayLength());
        Assert.Equal(1, json.GetProperty("RespostasObjetivas").GetArrayLength());
        Assert.Equal("Minha resposta discursiva", json.GetProperty("RespostasDiscursivas")[0].GetProperty("Resposta").GetString());
        Assert.Equal(1, json.GetProperty("ComentariosNoBlog").GetArrayLength());
        Assert.DoesNotContain("Authenticator", json.GetRawText());
    }

    [Fact]
    public async Task DeletingTheAccountErasesStudyDataButKeepsCommentsAnonymous()
    {
        var student = await StudentWithActivityAsync("privacidade.exclusao@gmail.com");
        var client = NewClient();
        await TestAccounts.LoginAsync(client, student.Email!);

        var response = await TestAccounts.SubmitFormAsync(client, "/Account/Manage/DeletePersonalData", "delete-user", new() { ["Input.Password"] = TestAccounts.Password });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Null(await db.Users.FirstOrDefaultAsync(u => u.Id == student.Id));
        Assert.False(await db.RoadmapProgress.AnyAsync(p => p.UserId == student.Id));
        Assert.False(await db.QuestionAttempts.AnyAsync(a => a.UserId == student.Id));
        Assert.False(await db.DiscursiveAnswers.AnyAsync(a => a.UserId == student.Id));
        Assert.False(await db.MockExams.AnyAsync(m => m.UserId == student.Id));

        var comment = await db.PostComments.SingleAsync(c => c.Text == $"Comentário de {student.Email}");
        Assert.Null(comment.UserId);
        var post = await db.Posts.SingleAsync(p => p.Id == comment.PostId);
        var html = WebUtility.HtmlDecode(await NewClient().GetStringAsync($"/blog/{post.Slug}"));
        Assert.Contains("Participante que excluiu a conta", html);
        Assert.Contains($"Comentário de {student.Email}", html);
    }

    /// <summary>Aluno com um pouco de tudo: roadmap, questão, discursiva, simulado e comentário.</summary>
    private async Task<ApplicationUser> StudentWithActivityAsync(string email)
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, email, Roles.Student);
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, $"autor.{Guid.NewGuid():N}@faculdade.edu.br", Roles.Student, Roles.Teacher);

        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        await services.GetRequiredService<RoadmapService>().SetStatusAsync(student.Id, "pilhas", NodeStatus.Done);
        await services.GetRequiredService<QuestionBank>().AnswerAsync(student.Id, 1, 'A');
        await services.GetRequiredService<QuestionBank>().AnswerDiscursiveAsync(student.Id, 8, "Minha resposta discursiva");
        await services.GetRequiredService<MockExamService>().StartAsync(student.Id, MockExamSize.Short);

        var blog = services.GetRequiredService<BlogService>();
        var post = await blog.CreateAsync(new Editor(teacher.Id, IsAdmin: false, IsTeacher: true), new PostDraft
        {
            Title = $"Post para {email}", Summary = "Resumo", Body = "Texto", Category = PostCategory.ClassAnnouncements,
        }, PostAction.Publish);
        await blog.AddCommentAsync(student.Id, post.Id!.Value, $"Comentário de {email}");

        return student;
    }
}
