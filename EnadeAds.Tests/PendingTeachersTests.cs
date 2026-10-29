using System.Net;
using EnadeAds.Accounts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class PendingTeachersTests(EnadeAdsFactory factory)
{
    private const string PageUrl = "/admin/professores-pendentes";

    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Anonymous_IsRedirectedToLogin()
    {
        var response = await NewClient().GetAsync(PageUrl);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Student_IsDenied()
    {
        await TestAccounts.CreateConfirmedAsync(factory, "aluno.negado@gmail.com", Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, "aluno.negado@gmail.com");

        var response = await client.GetAsync(PageUrl);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/AccessDenied", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Admin_ApprovesPendingTeacher()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "aprovar@faculdade.edu.br", Roles.Student, Roles.PendingTeacher);
        await TestAccounts.CreateConfirmedAsync(factory, "admin.aprova@faculdade.edu.br", Roles.Student, Roles.Admin);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, "admin.aprova@faculdade.edu.br");

        Assert.Contains("aprovar@faculdade.edu.br", await client.GetStringAsync(PageUrl));

        var response = await TestAccounts.SubmitFormAsync(client, PageUrl, "teacher-decision", new() { ["Command"] = $"aprovar:{teacher.Id}" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("agora é professor", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        Assert.Equal([Roles.Student, Roles.Teacher], (await TestAccounts.RolesOfAsync(factory, "aprovar@faculdade.edu.br")).Order());
    }

    [Fact]
    public async Task Reject_KeepsStudentOnly()
    {
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "recusar@faculdade.edu.br", Roles.Student, Roles.PendingTeacher);

        var result = await WithApprovals(approvals => approvals.RejectAsync(teacher.Id));

        Assert.Equal(TeacherDecisionResult.Done, result);
        Assert.Equal([Roles.Student], await TestAccounts.RolesOfAsync(factory, "recusar@faculdade.edu.br"));
    }

    [Fact]
    public async Task Approve_StudentWhoNeverAsked_IsRefused()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "nunca.pediu@faculdade.edu.br", Roles.Student);

        var result = await WithApprovals(approvals => approvals.ApproveAsync(student.Id));

        Assert.Equal(TeacherDecisionResult.NotPending, result);
        Assert.Equal([Roles.Student], await TestAccounts.RolesOfAsync(factory, "nunca.pediu@faculdade.edu.br"));
    }

    [Fact]
    public async Task Approve_UnknownUser_IsNotFound()
    {
        var result = await WithApprovals(approvals => approvals.ApproveAsync("nao-existe"));

        Assert.Equal(TeacherDecisionResult.UserNotFound, result);
    }

    private async Task<TeacherDecisionResult> WithApprovals(Func<TeacherApprovals, Task<TeacherDecisionResult>> action)
    {
        using var scope = factory.Services.CreateScope();
        return await action(scope.ServiceProvider.GetRequiredService<TeacherApprovals>());
    }
}
