using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Study;

/// <summary>Configuração que o admin muda pela tela, guardada no banco.</summary>
public class SiteSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>Datas de prova da trilha ativa. Cada trilha tem as suas, e o admin muda pela tela.</summary>
public sealed class ExamSettings(ApplicationDbContext db, TrackCatalog tracks)
{
    /// <summary>Próximo dia de prova a partir de <paramref name="today"/>; depois da prova, o último dia dela.</summary>
    public async Task<DateOnly> ExamDateAsync(DateOnly today, CancellationToken cancellationToken = default)
    {
        var track = await tracks.CurrentAsync(cancellationToken);
        return track.ExamDays.Where(day => day >= today).DefaultIfEmpty(track.LastExamDay).Min();
    }

    public async Task SetExamDatesAsync(DateOnly first, DateOnly? second = null, CancellationToken cancellationToken = default)
    {
        var current = await tracks.CurrentAsync(cancellationToken);
        var track = await db.Tracks.FirstAsync(t => t.Id == current.Id, cancellationToken);
        track.ExamDate = first;
        track.SecondExamDate = second;
        await db.SaveChangesAsync(cancellationToken);
    }
}
