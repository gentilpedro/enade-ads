using System.Text.RegularExpressions;

namespace EnadeAds.Questions;

/// <summary>
/// Código legível de uma questão importada do Inep, tirado da chave estável:
/// enade-2017-ads-q10 vira "2017·Q10" e enade-2021-ads-d03 vira "2021·D3".
/// Questões criadas na plataforma não têm código e seguem com o número.
/// </summary>
public static partial class QuestionCode
{
    public static string? From(string? externalKey)
    {
        var match = externalKey is null ? null : KeyPattern().Match(externalKey);
        return match is { Success: true }
            ? $"{match.Groups["year"].Value}·{char.ToUpperInvariant(match.Groups["kind"].Value[0])}{int.Parse(match.Groups["number"].Value)}"
            : null;
    }

    /// <summary>"2017·Q10" para questões do Inep, "nº 13" para as da plataforma.</summary>
    public static string Label(int id, string? externalKey) => From(externalKey) ?? $"nº {id}";

    [GeneratedRegex(@"^enade-(?<year>\d{4})-[a-z]+-(?<kind>[qd])(?<number>\d+)$")]
    private static partial Regex KeyPattern();
}
