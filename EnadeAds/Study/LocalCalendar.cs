using Microsoft.Extensions.Options;

namespace EnadeAds.Study;

/// <summary>"Hoje" no fuso da plataforma (o dos alunos), não no fuso do servidor, que em produção é UTC.</summary>
public sealed class LocalCalendar(TimeProvider clock, IOptions<ExamOptions> exam)
{
    public DateOnly Today()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(exam.Value.TimeZone);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), zone).DateTime);
    }
}
