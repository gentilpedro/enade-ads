using EnadeAds.Questions;
using EnadeAds.Study;

namespace EnadeAds.Tests;

public class QuestionSeedTests
{
    [Fact]
    public void EveryObjectiveHasFiveOptionsAToEWithExactlyOneCorrect()
    {
        Assert.All(QuestionSeed.Questions.Where(q => q.Kind == QuestionKind.Objective), question =>
        {
            var options = QuestionSeed.Options.Where(o => o.QuestionId == question.Id).OrderBy(o => o.Letter).ToList();

            Assert.Equal("ABCDE", new string(options.Select(o => o.Letter).ToArray()));
            Assert.Single(options, o => o.IsCorrect);
            Assert.All(options, o => Assert.False(string.IsNullOrWhiteSpace(o.Explanation)));
        });
    }

    [Fact]
    public void EveryDiscursiveHasExpectedAnswerAndNoOptions()
    {
        var discursives = QuestionSeed.Questions.Where(q => q.Kind == QuestionKind.Discursive).ToList();

        Assert.NotEmpty(discursives);
        Assert.All(discursives, question =>
        {
            Assert.False(string.IsNullOrWhiteSpace(question.ExpectedAnswer));
            Assert.InRange(question.SuggestedLines ?? 0, 1, QuestionEditor.MaxSuggestedLines);
            Assert.DoesNotContain(QuestionSeed.Options, o => o.QuestionId == question.Id);
        });
    }

    [Fact]
    public void ExpectedAnswerTextIsNotMistakenForCode()
    {
        var discursive = QuestionSeed.Questions.Single(q => q.Kind == QuestionKind.Discursive);

        Assert.All(StatementFormatter.Split(discursive.ExpectedAnswer!), block => Assert.False(block.IsCode));
    }

    [Theory]
    [InlineData(1, "estruturas-dados")]
    [InlineData(2, "arquitetura-computadores")]
    [InlineData(3, "redes-sistemas-distribuidos")]
    [InlineData(4, "banco-de-dados")]
    [InlineData(5, "engenharia-requisitos")]
    [InlineData(6, "qualidade-testes")]
    [InlineData(7, "orientacao-objetos")]
    [InlineData(8, "engenharia-requisitos")]
    public void QuestionPointsToTheRightSubject(int questionId, string subjectSlug)
    {
        var question = QuestionSeed.Questions.Single(q => q.Id == questionId);
        var subject = RoadmapSeed.Nodes.Single(n => n.Id == question.SubjectId);

        Assert.Equal(NodeKind.Subject, subject.Kind);
        Assert.Equal(subjectSlug, subject.Slug);
    }

    [Fact]
    public void OptionIdsAreUnique()
    {
        Assert.Equal(QuestionSeed.Options.Count, QuestionSeed.Options.Select(o => o.Id).Distinct().Count());
    }
}
