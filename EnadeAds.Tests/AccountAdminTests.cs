using System.Net;
using EnadeAds.Accounts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class AccountAdminTests(EnadeAdsFactory factory)
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task OnlyAdminsOpenTheAccountsPage()
    {
        await TestAccounts.CreateConfirmedAsync(factory, "contas.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);
        var teacher = NewClient();
        await TestAccounts.LoginAsync(teacher, "contas.prof@faculdade.edu.br");

        var response = await teacher.GetAsync("/admin/contas");

        Assert.Contains("/Account/AccessDenied", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Search_FindsByNameOrEmailAndFiltersByRole()
    {
        await TestAccounts.CreateConfirmedAsync(factory, "contas.busca.aluna@gmail.com", Roles.Student);
        await TestAccounts.CreateConfirmedAsync(factory, "contas.busca.prof@faculdade.edu.br", Roles.Student, Roles.Teacher);

        using var scope = factory.Services.CreateScope();
        var admin = scope.ServiceProvider.GetRequiredService<AccountAdmin>();
        var byEmail = await admin.SearchAsync("CONTAS.BUSCA", AccountFilter.All, 1);
        var teachers = await admin.SearchAsync("contas.busca", AccountFilter.Teachers, 1);
        var students = await admin.SearchAsync("contas.busca", AccountFilter.Students, 1);
        var byName = await admin.SearchAsync("teste contas.busca.aluna", AccountFilter.All, 1);

        Assert.Equal(2, byEmail.TotalCount);
        Assert.Equal("contas.busca.prof@faculdade.edu.br", Assert.Single(teachers.Items).Email);
        Assert.Equal("contas.busca.aluna@gmail.com", Assert.Single(students.Items).Email);
        Assert.Single(byName.Items);
    }

    [Fact]
    public async Task RevokeTeacher_KeepsTheStudentRole()
    {
        var admin = await TestAccounts.CreateConfirmedAsync(factory, "contas.admin1@faculdade.edu.br", Roles.Student, Roles.Admin);
        var teacher = await TestAccounts.CreateConfirmedAsync(factory, "contas.revoga@faculdade.edu.br", Roles.Student, Roles.Teacher);

        var result = await ApplyAsync(admin.Id, teacher.Id, AccountAction.RevokeTeacher);
        var again = await ApplyAsync(admin.Id, teacher.Id, AccountAction.RevokeTeacher);

        Assert.True(result.Succeeded);
        Assert.Equal([Roles.Student], await TestAccounts.RolesOfAsync(factory, teacher.Email!));
        Assert.False(again.Succeeded);
    }

    [Fact]
    public async Task BlockedAccountCannotLogInUntilUnblocked()
    {
        var admin = await TestAccounts.CreateConfirmedAsync(factory, "contas.admin2@faculdade.edu.br", Roles.Student, Roles.Admin);
        var student = await TestAccounts.CreateConfirmedAsync(factory, "contas.bloqueio@gmail.com", Roles.Student);

        await ApplyAsync(admin.Id, student.Id, AccountAction.Block);
        var blocked = await TryLoginAsync(student.Email!);
        await ApplyAsync(admin.Id, student.Id, AccountAction.Unblock);
        var unblocked = await TryLoginAsync(student.Email!);

        Assert.Contains("/Account/Lockout", blocked);
        Assert.DoesNotContain("/Account/Lockout", unblocked);
        Assert.DoesNotContain("Account/Login", unblocked);
    }

    [Fact]
    public async Task AdminCannotChangeOwnAccountButCanRemoveAnotherAdmin()
    {
        var admin = await TestAccounts.CreateConfirmedAsync(factory, "contas.admin3@faculdade.edu.br", Roles.Student, Roles.Admin);
        var other = await TestAccounts.CreateConfirmedAsync(factory, "contas.admin4@faculdade.edu.br", Roles.Student, Roles.Admin);

        var selfBlock = await ApplyAsync(admin.Id, admin.Id, AccountAction.Block);
        var selfRemove = await ApplyAsync(admin.Id, admin.Id, AccountAction.RemoveAdmin);
        var removeOther = await ApplyAsync(admin.Id, other.Id, AccountAction.RemoveAdmin);

        Assert.False(selfBlock.Succeeded);
        Assert.False(selfRemove.Succeeded);
        Assert.Contains(Roles.Admin, await TestAccounts.RolesOfAsync(factory, admin.Email!));
        Assert.True(removeOther.Succeeded);
        Assert.DoesNotContain(Roles.Admin, await TestAccounts.RolesOfAsync(factory, other.Email!));
    }

    [Fact]
    public async Task Page_AppliesActionsAndHidesThemOnOwnRow()
    {
        await TestAccounts.CreateConfirmedAsync(factory, "contas.pagina.admin@faculdade.edu.br", Roles.Student, Roles.Admin);
        var target = await TestAccounts.CreateConfirmedAsync(factory, "contas.pagina.alvo@gmail.com", Roles.Student);
        var client = NewClient();
        await TestAccounts.LoginAsync(client, "contas.pagina.admin@faculdade.edu.br");

        var own = WebUtility.HtmlDecode(await client.GetStringAsync("/admin/contas?busca=contas.pagina.admin"));
        var url = "/admin/contas?busca=contas.pagina.alvo";
        var response = await TestAccounts.SubmitFormAsync(client, url, "account-action", new() { ["Command"] = $"Block:{target.Id}" });
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Contains("você", own);
        Assert.DoesNotContain("Bloquear", own);
        Assert.Contains("foi bloqueado", html);
        Assert.Contains("Bloqueada", html);
    }

    private async Task<EditResultMessage> ApplyAsync(string adminId, string targetId, AccountAction action)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AccountAdmin>().ApplyAsync(adminId, targetId, action);
    }

    /// <summary>Tenta entrar e devolve para onde o login redirecionou.</summary>
    private async Task<string> TryLoginAsync(string email)
    {
        var client = NewClient();
        var response = await TestAccounts.SubmitFormAsync(client, "/Account/Login", "login", new()
        {
            ["Input.Email"] = email,
            ["Input.Password"] = TestAccounts.Password,
        });
        return response.Headers.Location?.ToString() ?? "";
    }
}
