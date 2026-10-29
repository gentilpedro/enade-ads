namespace EnadeAds.Accounts;

public static class Roles
{
    public const string Student = "Aluno";
    public const string PendingTeacher = "ProfessorPendente";
    public const string Teacher = "Professor";
    public const string Admin = "Admin";

    public static readonly IReadOnlyList<string> All = [Student, PendingTeacher, Teacher, Admin];
}
