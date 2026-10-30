using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnadeAds.Study;

/// <summary>Configuração que o admin muda pela tela, guardada no banco.</summary>
public class SiteSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>Data da prova: a que o admin gravou no banco ou, se nunca gravou, a da configuração (<c>Enade:ExamDate</c>).</summary>
public sealed class ExamSettings(ApplicationDbContext db, IOptions<ExamOptions> exam, TimeProvider clock)
{
    public const string ExamDateKey = "ExamDate";

    public async Task<DateOnly> ExamDateAsync(CancellationToken cancellationToken = default)
    {
        var stored = await db.SiteSettings.AsNoTracking()
            .Where(s => s.Key == ExamDateKey)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return DateOnly.TryParseExact(stored, "yyyy-MM-dd", out var date) ? date : exam.Value.ExamDate;
    }

    public async Task SetExamDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var setting = await db.SiteSettings.FirstOrDefaultAsync(s => s.Key == ExamDateKey, cancellationToken);
        var value = date.ToString("yyyy-MM-dd");
        if (setting is null)
        {
            db.SiteSettings.Add(new SiteSetting { Key = ExamDateKey, Value = value, UpdatedAt = clock.GetUtcNow() });
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = clock.GetUtcNow();
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
