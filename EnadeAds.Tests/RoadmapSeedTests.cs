using EnadeAds.Study;

namespace EnadeAds.Tests;

public class RoadmapSeedTests
{
    private static readonly IReadOnlyList<RoadmapNode> Nodes = RoadmapSeed.Nodes;

    [Fact]
    public void HasThe20PortariaSubjectsPlusGeneralEducation()
    {
        var subjects = Nodes.Where(n => n.Kind == NodeKind.Subject).ToList();

        Assert.Equal(21, subjects.Count);
        Assert.Equal(20, subjects.Count(s => s.Block != StudyBlock.GeneralEducation));
    }

    [Fact]
    public void IdsAndSlugsAreUnique()
    {
        Assert.Equal(Nodes.Count, Nodes.Select(n => n.Id).Distinct().Count());
        Assert.Equal(Nodes.Count, Nodes.Select(n => n.Slug).Distinct().Count());
    }

    [Fact]
    public void EverySubjectHasTopicsAndEveryTopicHasASubject()
    {
        var subjectIds = Nodes.Where(n => n.Kind == NodeKind.Subject).Select(n => n.Id).ToHashSet();

        Assert.All(subjectIds, id => Assert.Contains(Nodes, n => n.ParentId == id));
        Assert.All(Nodes.Where(n => n.Kind == NodeKind.Topic), topic => Assert.Contains(topic.ParentId!.Value, subjectIds));
    }

    [Fact]
    public void SpineOrderIsSequentialAndEndsWithTheMockExam()
    {
        var spine = Nodes.Where(n => n.ParentId is null).OrderBy(n => n.Order).ToList();

        Assert.Equal(Enumerable.Range(1, spine.Count), spine.Select(n => n.Order));
        Assert.Equal("checkpoint-simulado", spine[^1].Slug);
    }

    [Fact]
    public void SlugsAreUrlSafe()
    {
        Assert.All(Nodes, n => Assert.Matches("^[a-z0-9]+(-[a-z0-9]+)*$", n.Slug));
    }
}
