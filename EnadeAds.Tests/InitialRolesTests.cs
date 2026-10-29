using EnadeAds.Accounts;

namespace EnadeAds.Tests;

public class InitialRolesTests
{
    private static readonly string[] NoAdmins = [];

    [Fact]
    public void Student_GetsOnlyStudentRole()
    {
        var roles = InitialRoles.For("aluno@gmail.com", wantsTeacherAccess: false, NoAdmins);

        Assert.Equal([Roles.Student], roles);
    }

    [Fact]
    public void InstitutionalEmailWithoutCheckbox_IsStudent()
    {
        var roles = InitialRoles.For("aluno@faculdade.edu.br", wantsTeacherAccess: false, NoAdmins);

        Assert.Equal([Roles.Student], roles);
    }

    [Fact]
    public void TeacherWithInstitutionalEmail_IsPendingNotTeacher()
    {
        var roles = InitialRoles.For("prof@faculdade.edu.br", wantsTeacherAccess: true, NoAdmins);

        Assert.Equal([Roles.Student, Roles.PendingTeacher], roles);
        Assert.DoesNotContain(Roles.Teacher, roles);
    }

    [Fact]
    public void TeacherWithoutInstitutionalEmail_IsRejected()
    {
        var error = Assert.Throws<ArgumentException>(() =>
            InitialRoles.For("prof@gmail.com", wantsTeacherAccess: true, NoAdmins));

        Assert.StartsWith(InitialRoles.TeacherRequiresInstitutionalEmail, error.Message);
    }

    [Fact]
    public void ConfiguredAdminEmail_GetsAdminRole()
    {
        var roles = InitialRoles.For(" Admin@Exemplo.com ", wantsTeacherAccess: false, ["admin@exemplo.com"]);

        Assert.Contains(Roles.Admin, roles);
    }
}
