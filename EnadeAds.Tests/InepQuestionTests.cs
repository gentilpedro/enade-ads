using System.Text.Json;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnadeAds.Tests;

/// <summary>Confere os arquivos de Data/Seed/inep contra os gabaritos oficiais publicados pelo Inep.</summary>
public class InepSeedFileTests
{
    // Gabaritos definitivos do Inep (2021: 2021_GB_tecnologia_analise_desenvolvimento_sistemas.pdf;
    // 2017: 41_CST_Analise_e_Desenvolvimento_de_Sistemas.pdf). '-' = anulada.
    private static readonly Dictionary<int, string> AnswerKeys = new()
    {
        [2021] = "ECBBAACDCDECCEBECAAEBABDEECBDCDBDAA",
        [2017] = "CCBBCEADBCCDCC-ADACEDBBEAADEDABCDBC",
    };

    private static string SeedFolder => Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "inep");

    private static string WwwRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "EnadeAds", "wwwroot"));

    private static IEnumerable<(int Year, SeedQuestion Question)> AllQuestions() =>
        Directory.EnumerateFiles(SeedFolder, "*.json")
            .Select(file => JsonSerializer.Deserialize<SeedExam>(File.ReadAllText(file), InepQuestionImporter.JsonOptions)!)
            .SelectMany(exam => exam.Questions.Select(q => (exam.Year, q)));

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
        var subjects = RoadmapSeed.Nodes.Where(n => n.Kind == NodeKind.Subject).ToDictionary(n => n.Slug, n => n.Id);

        Assert.All(AllQuestions(), item => Assert.Null(InepQuestionImporter.Validate(item.Question, subjects)));
    }

    [Fact]
    public void ObjectiveAnswersMatchTheOfficialAnswerKey()
    {
        Assert.All(AllQuestions().Where(q => q.Question.Kind == QuestionKind.Objective), item =>
        {
            var number = int.Parse(item.Question.Key[^2..]);
            var official = AnswerKeys[item.Year][number - 1];

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
            Assert.Contains(generalEducation ? "Formação Geral" : "Análise e Desenvolvimento de Sistemas", item.Question.Source);
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
