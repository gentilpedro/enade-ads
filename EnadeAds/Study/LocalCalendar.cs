using Microsoft.Extensions.Options;

namespace EnadeAds.Study;

/// <summary>"Hoje" no fuso da plataforma (o dos alunos), não no fuso do servidor, que em produção é UTC.</summary>
public sealed class LocalCalendar(TimeProvider clock, IOptions<ExamOptions> exam)
{
    public DateOnly Today() => DateOnly.FromDateTime(ToLocal(clock.GetUtcNow()));

    /// <summary>Converte um instante para a hora local da plataforma, para exibir datas.</summary>
    public DateTime ToLocal(DateTimeOffset moment) =>
        TimeZoneInfo.ConvertTime(moment, TimeZoneInfo.FindSystemTimeZoneById(exam.Value.TimeZone)).DateTime;
}
