using EnadeAds.Accounts;
using EnadeAds.Data;
using EnadeAds.MockExams;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

public class RedesRoadmapSeedTests
{
    private static readonly IReadOnlyList<RoadmapNode> Nodes = RedesRoadmapSeed.Nodes;

    [Fact]
    public void CoversTheTwentyObjectsOfThePortariaAndGeneralEducation()
    {
        var subjects = Nodes.Where(n => n.Kind == NodeKind.Subject).ToList();

        // Portaria Inep nº 172/2026, Art. 6º: 20 objetos de conhecimento; mais a Formação Geral.
        Assert.Equal(21, subjects.Count);
        Assert.Contains(subjects, s => s.Slug == MockExamService.GeneralEducationSlug && s.Block == StudyBlock.GeneralEducation);
        Assert.Equal(
            [StudyBlock.Fundamentals, StudyBlock.NetworkTechnologies, StudyBlock.NetworkOperations, StudyBlock.NetworkDesign, StudyBlock.GeneralEducation],
            subjects.Select(s => s.Block).Distinct());
        Assert.All(subjects, s => Assert.Contains(Nodes, t => t.ParentId == s.Id));
        Assert.All(Nodes, n => Assert.Equal(Tracks.EnadeRedes, n.TrackId));
    }

    [Fact]
    public void IdsNeverCrossTheOtherTracks()
    {
        var all = RoadmapSeed.Nodes.Concat(EnemRoadmapSeed.Nodes).Concat(Nodes).ToList();

        Assert.Equal(all.Count, all.Select(n => n.Id).Distinct().Count());
        Assert.All(Nodes, n => Assert.True(n.Id > RedesRoadmapSeed.IdBase));
        Assert.Equal(Nodes.Count, Nodes.Select(n => n.Slug).Distinct().Count());
    }
}

[Collection(AppCollection.Name)]
public class RedesTrackTests(EnadeAdsFactory factory)
{
    private (IServiceScope Scope, ApplicationDbContext Db) RedesScope()
    {
        var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<TrackScope>().Use(Tracks.EnadeRedes);
        return (scope, scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }

    [Fact]
    public async Task Redes_GetsItsOwnQuestionsAndACopyOfTheGeneralEducation()
    {
        var (scope, redes) = RedesScope();
        using (scope)
        {
            using var adsScope = factory.Services.CreateScope();
            var ads = adsScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var redesKeys = await redes.Questions.Select(q => q.ExternalKey!).ToListAsync();
            var adsGeneral = await ads.Questions.Where(q => q.Subject!.Slug == MockExamService.GeneralEducationSlug).Select(q => q.ExternalKey!).ToListAsync();
            var redesGeneral = await redes.Questions.Where(q => q.Subject!.Slug == MockExamService.GeneralEducationSlug).Select(q => q.ExternalKey!).ToListAsync();

            Assert.Contains(redesKeys, k => k.StartsWith("enade-2021-redes-"));
            Assert.Contains(redesKeys, k => k.StartsWith("enade-2017-redes-"));
            Assert.DoesNotContain(redesKeys, k => k.StartsWith("enade-2021-ads-") && !k.EndsWith("@enade-redes"));
            // Cada trilha tem a sua cópia: responder ou corrigir a questão numa não mexe na outra.
            Assert.Equal(adsGeneral.Select(k => InepQuestionImporter.SharedKey(k, "enade-redes")).Order(), redesGeneral.Order());
        }
    }

    [Fact]
    public async Task RedesMockExam_FollowsTheEnadeFormat()
    {
        var student = await TestAccounts.CreateConfirmedAsync(factory, "redes.simulado@gmail.com", Roles.Student);

        var (scope, _) = RedesScope();
        using (scope)
        {
            var exams = scope.ServiceProvider.GetRequiredService<MockExamService>();
            var started = await exams.StartAsync(student.Id, MockExamSize.Short);
            var exam = await exams.FindAsync(student.Id, started.MockExamId!.Value);
            var format = MockExamFormat.Of(exam!);

            Assert.Equal(StartOutcome.Started, started.Outcome);
            Assert.Equal(Tracks.EnadeRedes, exam!.TrackId);
            Assert.Equal(format.GeneralEducation, exam.Questions.Count(q => q.IsGeneralEducation));
            Assert.Equal(format.Discursive, exam.Questions.Count(q => q.Question!.Kind == QuestionKind.Discursive));
            Assert.All(exam.Questions, q => Assert.Equal(Tracks.EnadeRedes, q.Question!.Subject!.TrackId));
        }
    }
}
