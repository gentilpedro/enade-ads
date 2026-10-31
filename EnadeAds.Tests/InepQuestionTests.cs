using System.Text.Json;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

/// <summary>Confere os arquivos de Data/Seed/inep (ADS) e Data/Seed/redes contra os gabaritos oficiais publicados pelo Inep.</summary>
public class InepSeedFileTests
{
    // Gabaritos definitivos do Inep, questões 1 a 35 ('-' = anulada). As 8 primeiras são da Formação Geral, igual nos dois cursos.
    // ADS 2021: 2021_GB_tecnologia_analise_desenvolvimento_sistemas.pdf; ADS 2017: 41_CST_Analise_e_Desenvolvimento_de_Sistemas.pdf.
    // Redes 2021: 2021_GB_tecnologia_redes_computadores.pdf; Redes 2017: 44_CST_em_Redes_de_Computadores.pdf.
    private static readonly Dictionary<(string Track, int Year), string> AnswerKeys = new()
    {
        [("enade-ads", 2021)] = "ECBBAACDCDECCEBECAAEBABDEECBDCDBDAA",
        [("enade-ads", 2017)] = "CCBBCEADBCCDCC-ADACEDBBEAADEDABCDBC",
        [("enade-redes", 2021)] = "ECBBAACDCCBBBDBEBDDBAAECEADEACACADC",
        [("enade-redes", 2017)] = "CCBBCEADBEBEDCADACBB-EBBDDBAEEEBCCA",
    };

    private static readonly Dictionary<string, (string Course, IReadOnlyList<RoadmapNode> Roadmap)> Courses = new()
    {
        ["enade-ads"] = ("Análise e Desenvolvimento de Sistemas", RoadmapSeed.Nodes),
        ["enade-redes"] = ("Tecnologia em Redes de Computadores", RedesRoadmapSeed.Nodes),
    };

    private static string WwwRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "EnadeAds", "wwwroot"));

    private static IEnumerable<(string Track, int Year, SeedQuestion Question)> AllQuestions() =>
        new[] { "inep", "redes" }
            .SelectMany(folder => Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, "Data", "Seed", folder), "*.json"))
            .Select(file => JsonSerializer.Deserialize<SeedExam>(File.ReadAllText(file), InepQuestionImporter.JsonOptions)!)
            .SelectMany(exam => exam.Questions.Select(q => (exam.Track ?? "enade-ads", exam.Year, q)));

    /// <summary>As trilhas que recebem a questão: a do arquivo e as que compartilham a Formação Geral.</summary>
    private static IEnumerable<(string Track, int Year, SeedQuestion Question)> AsImported() =>
        new[] { "inep", "redes" }
            .SelectMany(folder => Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, "Data", "Seed", folder), "*.json"))
            .Select(file => JsonSerializer.Deserialize<SeedExam>(File.ReadAllText(file), InepQuestionImporter.JsonOptions)!)
            .SelectMany(exam => new[] { exam.Track ?? "enade-ads" }.Concat(exam.SharedWith ?? [])
                .SelectMany(track => exam.Questions.Select(q => (track, exam.Year, q))));

    [Fact]
    public void EveryEnadeTrackHasBothEditions()
    {
        foreach (var track in Courses.Keys)
        {
            Assert.Contains(AllQuestions(), q => q.Track == track && q.Year == 2017);
            Assert.Contains(AllQuestions(), q => q.Track == track && q.Year == 2021);
        }
    }

    [Fact]
    public void SeedFilesExistAndKeysAreUnique()
    {
        var keys = AllQuestions().Select(q => q.Question.Key).ToList();

        Assert.NotEmpty(keys);
        Assert.Equal(keys.Count, keys.Distinct().Count());
    }

    [Fact]
    public void EveryQuestionIsValidForTheImporter()
    {
        // Inclusive a Formação Geral na trilha de Redes: a matéria precisa existir no roadmap de cada trilha que recebe a questão.
        Assert.All(AsImported(), item =>
        {
            var subjects = Courses[item.Track].Roadmap.Where(n => n.Kind == NodeKind.Subject).ToDictionary(n => n.Slug, n => n.Id);
            Assert.Null(InepQuestionImporter.Validate(item.Question, subjects));
        });
    }

    [Fact]
    public void ObjectiveAnswersMatchTheOfficialAnswerKey()
    {
        Assert.All(AllQuestions().Where(q => q.Question.Kind == QuestionKind.Objective), item =>
        {
            var number = int.Parse(item.Question.Key[^2..]);
            var official = AnswerKeys[(item.Track, item.Year)][number - 1];

            Assert.NotEqual('-', official);
            Assert.Equal($"{official}", item.Question.Correct);
            Assert.StartsWith("Correta", item.Question.Options![official - 'A'].Explanation);
        });
    }

    [Fact]
    public void GeneralEducationQuestionsUseTheGeneralEducationSubject()
    {
        Assert.All(AllQuestions(), item =>
        {
            var number = int.Parse(item.Question.Key[^2..]);
            var discursive = item.Question.Kind == QuestionKind.Discursive;
            var generalEducation = discursive ? number <= 2 : number <= 8;

            Assert.Equal(generalEducation, item.Question.Subject == "formacao-geral");
            Assert.Contains(generalEducation ? "Formação Geral" : Courses[item.Track].Course, item.Question.Source);
        });
    }

    [Fact]
    public void DiscursivesSayTheExpectedAnswerIsNotOfficial()
    {
        Assert.All(AllQuestions().Where(q => q.Question.Kind == QuestionKind.Discursive), item =>
            Assert.StartsWith("Padrão de resposta elaborado pela plataforma (não é o padrão oficial do Inep).", item.Question.ExpectedAnswer));
    }

    [Fact]
    public void EveryReferencedImageExistsAndRenders()
    {
        Assert.All(AllQuestions(), item =>
        {
            var html = StatementFormatter.ToHtml(item.Question.Statement);
            foreach (var path in System.Text.RegularExpressions.Regex.Matches(item.Question.Statement, @"\]\((img/[^)\s]+)\)").Select(m => m.Groups[1].Value))
            {
                Assert.True(File.Exists(Path.Combine(WwwRoot, path)), $"{item.Question.Key}: imagem {path} não existe");
                Assert.Contains($"src=\"{path}\"", html);
            }
        });
    }
}

[Collection(AppCollection.Name)]
public class InepImporterTests(EnadeAdsFactory factory)
{
    [Fact]
    public async Task ImportIsIdempotentAndNeverOverwritesEdits()
    {
        var seed = new SeedQuestion("teste-importacao-q01", "Fonte de teste", QuestionKind.Objective, "estruturas-dados", "Enunciado original",
            [new("A", "Correta. a"), new("B", "b"), new("C", "c"), new("D", "d"), new("E", "e")], "A", null, null);

        using var scope = factory.Services.CreateScope();
        var importer = scope.ServiceProvider.GetRequiredService<InepQuestionImporter>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var first = await importer.ImportAsync([seed], "teste");
        var question = await db.Questions.SingleAsync(q => q.ExternalKey == "teste-importacao-q01");
        question.Statement = "Enunciado corrigido pelo professor";
        await db.SaveChangesAsync();
        var second = await importer.ImportAsync([seed], "teste");

        Assert.Equal(1, first);
        Assert.Equal(0, second);
        Assert.Equal("Enunciado corrigido pelo professor", (await db.Questions.AsNoTracking().SingleAsync(q => q.ExternalKey == "teste-importacao-q01")).Statement);
    }

    [Fact]
    public async Task InvalidQuestionIsSkippedNotFatal()
    {
        var broken = new SeedQuestion("teste-importacao-q02", "Fonte", QuestionKind.Objective, "materia-que-nao-existe", "Enunciado",
            [new("A", "a")], "A", null, null);

        using var scope = factory.Services.CreateScope();
        var imported = await scope.ServiceProvider.GetRequiredService<InepQuestionImporter>().ImportAsync([broken], "teste");

        Assert.Equal(0, imported);
    }

    [Fact]
    public async Task SeedFilesAreImportedOnStartup()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var imported = await db.Questions.CountAsync(q => q.ExternalKey != null && q.ExternalKey.StartsWith("enade-"));

        Assert.True(imported > 0);
    }
}
