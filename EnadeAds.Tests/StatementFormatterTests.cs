using EnadeAds.Questions;

namespace EnadeAds.Tests;

public class StatementFormatterTests
{
    [Fact]
    public void SplitsParagraphsAndCodeBlocks()
    {
        var blocks = StatementFormatter.Split("""
            Considere o código:

            for (int i = 0; i < 3; i++) {
                print(i);
            }

            O que é impresso?
            """);

        Assert.Equal(3, blocks.Count);
        Assert.False(blocks[0].IsCode);
        Assert.True(blocks[1].IsCode);
        Assert.Contains("    print(i);", blocks[1].Text);
        Assert.False(blocks[2].IsCode);
    }

    [Fact]
    public void AlignedTableAndSqlAreCode()
    {
        var blocks = StatementFormatter.Split("id | nome\n 1 | ADS\n\nSELECT nome\nFROM Curso;");

        Assert.All(blocks, block => Assert.True(block.IsCode));
    }

    [Fact]
    public void PlainSentenceIsParagraph()
    {
        var blocks = StatementFormatter.Split("Qual dos itens abaixo é um requisito não funcional?");

        Assert.False(Assert.Single(blocks).IsCode);
    }
}
