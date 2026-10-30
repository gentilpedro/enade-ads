using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace EnadeAds.Questions;

public sealed record StatementBlock(string Text, bool IsCode);

/// <summary>
/// Converte o enunciado (texto com marcação mínima) em HTML seguro. Todo texto é escapado antes de
/// receber as poucas tags permitidas, então nenhum HTML digitado pelo professor chega à página.
///
/// Marcação aceita:
/// - linha em branco separa parágrafos;
/// - bloco entre linhas <c>```</c> vira código pré-formatado;
/// - <c>**negrito**</c>, <c>`código`</c> e <c>![descrição](https://…)</c> dentro de parágrafos;
/// - sem cercas, blocos com cara de código (indentação, tabelas, chaves, <c>;</c>) também viram código,
///   para manter o conteúdo antigo legível.
/// </summary>
public static partial class StatementFormatter
{
    private const string Fence = "```";

    public static IReadOnlyList<StatementBlock> Split(string statement)
    {
        var blocks = new List<StatementBlock>();
        var current = new List<string>();
        var inFence = false;

        foreach (var rawLine in statement.Replace("\r\n", "\n").Split('\n'))
        {
            var line = rawLine.TrimEnd();

            if (line.TrimStart().StartsWith(Fence, StringComparison.Ordinal))
            {
                if (inFence)
                {
                    blocks.Add(new StatementBlock(string.Join('\n', current), IsCode: true));
                    current.Clear();
                }
                else
                {
                    Flush();
                }

                inFence = !inFence;
                continue;
            }

            if (inFence)
            {
                current.Add(line);
            }
            else if (line.Length == 0)
            {
                Flush();
            }
            else
            {
                current.Add(line);
            }
        }

        // Cerca aberta e não fechada: trata o resto como código em vez de perder o texto.
        if (inFence)
        {
            blocks.Add(new StatementBlock(string.Join('\n', current), IsCode: true));
        }
        else
        {
            Flush();
        }

        return blocks;

        void Flush()
        {
            if (current.Count == 0)
            {
                return;
            }

            blocks.Add(new StatementBlock(string.Join('\n', current), current.Any(LooksLikeCode)));
            current.Clear();
        }
    }

    public static string ToHtml(string statement)
    {
        var html = new StringBuilder();
        foreach (var block in Split(statement))
        {
            if (block.IsCode)
            {
                html.Append("<pre><code>").Append(HtmlEncoder.Default.Encode(block.Text)).Append("</code></pre>");
            }
            else
            {
                html.Append("<p>").Append(Inline(block.Text)).Append("</p>");
            }
        }

        return html.ToString();
    }

    private static string Inline(string text)
    {
        var html = new StringBuilder();
        var position = 0;

        foreach (Match match in InlineToken().Matches(text))
        {
            html.Append(Encode(text[position..match.Index]));

            if (match.Groups["bold"].Success)
            {
                html.Append("<strong>").Append(Encode(match.Groups["bold"].Value)).Append("</strong>");
            }
            else if (match.Groups["code"].Success)
            {
                html.Append("<code>").Append(Encode(match.Groups["code"].Value)).Append("</code>");
            }
            else if (Uri.TryCreate(match.Groups["src"].Value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps)
            {
                html.Append("<img src=\"").Append(HtmlEncoder.Default.Encode(uri.AbsoluteUri))
                    .Append("\" alt=\"").Append(HtmlEncoder.Default.Encode(match.Groups["alt"].Value))
                    .Append("\" loading=\"lazy\" class=\"img-fluid my-2\" />");
            }
            else
            {
                // Imagem sem https (ou com URL inválida) aparece como texto, sem virar tag.
                html.Append(Encode(match.Value));
            }

            position = match.Index + match.Length;
        }

        html.Append(Encode(text[position..]));
        return html.ToString();
    }

    private static string Encode(string text) => HtmlEncoder.Default.Encode(text).Replace("&#xA;", "<br />");

    private static bool LooksLikeCode(string line) =>
        line.StartsWith("  ", StringComparison.Ordinal)
        || line.Contains("  ", StringComparison.Ordinal)
        || line.Contains(" | ", StringComparison.Ordinal)
        || line.Contains('{') || line.Contains('}')
        || line.TrimEnd().EndsWith(';');

    [GeneratedRegex(@"\*\*(?<bold>[^*\n]+)\*\*|`(?<code>[^`\n]+)`|!\[(?<alt>[^\]\n]*)\]\((?<src>[^)\s]+)\)")]
    private static partial Regex InlineToken();
}
