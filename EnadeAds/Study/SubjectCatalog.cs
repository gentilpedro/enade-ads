using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Study;

public sealed class SubjectCatalog(ApplicationDbContext db)
{
    public async Task<IReadOnlyList<RoadmapNode>> ListAsync(CancellationToken cancellationToken = default) =>
        await db.RoadmapNodes.AsNoTracking()
            .Where(n => n.Kind == NodeKind.Subject)
            .Include(n => n.Topics)
            .OrderBy(n => n.Order)
            .ToListAsync(cancellationToken);

    public async Task<RoadmapNode?> FindAsync(string slug, CancellationToken cancellationToken = default)
    {
        var subject = await db.RoadmapNodes.AsNoTracking()
            .Include(n => n.Topics)
            .FirstOrDefaultAsync(n => n.Slug == slug && n.Kind == NodeKind.Subject, cancellationToken);

        subject?.Topics.Sort((a, b) => a.Order.CompareTo(b.Order));
        return subject;
    }
}
