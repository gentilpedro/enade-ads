namespace EnadeAds.Accounts;

/// <summary>
/// E-mail institucional tem "edu" como rótulo do domínio depois do nome da instituição:
/// nome@faculdade.edu.br e nome@mit.edu contam; nome@educacao.com.br e nome@edu.exemplo.com não.
/// </summary>
public static class InstitutionalEmail
{
    private const string InstitutionalLabel = "edu";

    public static bool IsInstitutional(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var at = email.LastIndexOf('@');
        if (at <= 0 || at == email.Length - 1)
        {
            return false;
        }

        var labels = email[(at + 1)..].Trim().TrimEnd('.').Split('.');

        // O primeiro rótulo é o nome da instituição; "edu" só vale como sufixo (edu, edu.br, edu.pt...).
        return labels.Length >= 2
            && labels.Skip(1).Any(label => label.Equals(InstitutionalLabel, StringComparison.OrdinalIgnoreCase));
    }
}
