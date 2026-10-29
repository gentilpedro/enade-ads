namespace EnadeAds.Accounts;

public static class Roles
{
    public const string Student = "Aluno";
    public const string PendingTeacher = "ProfessorPendente";
    public const string Teacher = "Professor";
    public const string Admin = "Admin";

    /// <summary>Quem publica conteúdo (questões, desafios, posts), para usar em [Authorize(Roles = ...)].</summary>
    public const string Editors = Teacher + "," + Admin;

    public static readonly IReadOnlyList<string> All = [Student, PendingTeacher, Teacher, Admin];
}
