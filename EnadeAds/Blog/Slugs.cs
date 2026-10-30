using System.Globalization;
using System.Text;

namespace EnadeAds.Blog;

public static class Slugs
{
    public const int MaxLength = 80;

    /// <summary>Segmentos de /blog/... que já são páginas: um post não pode ter esse endereço.</summary>
    private static readonly HashSet<string> Reserved = ["novo"];

    public static bool IsReserved(string slug) => Reserved.Contains(slug);

    /// <summary>"Edital do ENADE 2026: o que muda?" → "edital-do-enade-2026-o-que-muda".</summary>
    public static string From(string title)
    {
        var builder = new StringBuilder();
        foreach (var c in title.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(c))
            {
                builder.Append(char.ToLowerInvariant(c));
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        var slug = builder.ToString().Trim('-');
        if (slug.Length > MaxLength)
        {
            slug = slug[..MaxLength].TrimEnd('-');
        }

        return slug.Length == 0 ? "post" : slug;
    }
}
