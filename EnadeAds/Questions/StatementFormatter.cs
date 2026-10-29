namespace EnadeAds.Questions;

public sealed record StatementBlock(string Text, bool IsCode);

/// <summary>
/// Divide o enunciado (texto puro) em blocos separados por linha em branco. Blocos com código, SQL ou
/// tabelas alinhadas com espaços viram texto pré-formatado; o resto vira parágrafo.
/// </summary>
public static class StatementFormatter
{
    public static IReadOnlyList<StatementBlock> Split(string statement)
    {
        var blocks = new List<StatementBlock>();
        var current = new List<string>();

        foreach (var line in statement.Replace("\r\n", "\n").Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                Flush();
            }
            else
            {
                current.Add(line.TrimEnd());
            }
        }

        Flush();
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

    private static bool LooksLikeCode(string line) =>
        line.StartsWith("  ", StringComparison.Ordinal)
        || line.Contains("  ", StringComparison.Ordinal)
        || line.Contains(" | ", StringComparison.Ordinal)
        || line.Contains('{') || line.Contains('}')
        || line.TrimEnd().EndsWith(';');
}
