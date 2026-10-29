namespace EnadeAds.Accounts;

public static class InitialRoles
{
    public const string TeacherRequiresInstitutionalEmail =
        "Para se cadastrar como professor, use seu e-mail institucional (domínio com .edu, por exemplo nome@faculdade.edu.br).";

    /// <summary>
    /// Perfis de uma conta recém-criada. Quem pede acesso de professor fica pendente até um admin aprovar,
    /// porque alunos também podem ter e-mail institucional.
    /// </summary>
    public static IReadOnlyList<string> For(string email, bool wantsTeacherAccess, IEnumerable<string> adminEmails)
    {
        if (wantsTeacherAccess && !InstitutionalEmail.IsInstitutional(email))
        {
            throw new ArgumentException(TeacherRequiresInstitutionalEmail, nameof(email));
        }

        var roles = new List<string> { Roles.Student };

        if (wantsTeacherAccess)
        {
            roles.Add(Roles.PendingTeacher);
        }

        if (adminEmails.Any(admin => admin.Trim().Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            roles.Add(Roles.Admin);
        }

        return roles;
    }
}
