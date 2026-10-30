using EnadeAds.Study;

namespace EnadeAds.Tests;

public class SemesterPlanTests
{
    private static readonly DateOnly Thursday = new(2026, 10, 29);
    private static readonly DateOnly ExamSunday = new(2026, 11, 29);

    [Fact]
    public void Weeks_RunMondayToSundayFromThisWeekToTheExamWeek()
    {
        var weeks = SemesterPlan.Weeks(Thursday, ExamSunday, maxColumns: 6, out var skipped);

        Assert.Equal(0, skipped);
        Assert.Equal(5, weeks.Count);
        Assert.Equal(new DateOnly(2026, 10, 26), weeks[0].Start);
        Assert.Equal(DayOfWeek.Monday, weeks[0].Start.DayOfWeek);
        Assert.Equal(ExamSunday, weeks[^1].End);
        Assert.Equal(44, weeks[0].Number);
    }

    [Fact]
    public void Weeks_KeepTheFirstOnesAndTheExamWeekWhenTheyDoNotFit()
    {
        var weeks = SemesterPlan.Weeks(Thursday, ExamSunday, maxColumns: 3, out var skipped);

        Assert.Equal(2, skipped);
        Assert.Equal(new[] { new DateOnly(2026, 10, 26), new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 23) }, weeks.Select(w => w.Start));
    }

    [Fact]
    public void Weeks_OnExamDayIsASingleColumn()
    {
        var weeks = SemesterPlan.Weeks(ExamSunday, ExamSunday, maxColumns: 6, out var skipped);

        Assert.Equal(0, skipped);
        Assert.Single(weeks);
    }
}

public class QuestionCodeTests
{
    [Theory]
    [InlineData("enade-2017-ads-q10", "2017·Q10")]
    [InlineData("enade-2021-ads-d03", "2021·D3")]
    [InlineData("outra-coisa", null)]
    [InlineData(null, null)]
    public void Code_ComesFromTheInepKey(string? key, string? expected)
    {
        Assert.Equal(expected, EnadeAds.Questions.QuestionCode.From(key));
    }

    [Fact]
    public void Label_FallsBackToThePlatformNumber()
    {
        Assert.Equal("nº 2", EnadeAds.Questions.QuestionCode.Label(2, null));
    }
}
