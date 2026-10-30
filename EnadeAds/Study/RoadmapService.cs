using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnadeAds.Study;

public sealed record RoadmapItem(RoadmapNode Node, NodeStatus? Status, IReadOnlyList<RoadmapItem> Topics);

public sealed record RoadmapCounts(int Total, int Done, int InProgress, int Skipped)
{
    public int Percent => Total == 0 ? 0 : (int)Math.Round(100.0 * Done / Total);
}

public sealed record RoadmapView(IReadOnlyList<RoadmapItem> Spine, RoadmapCounts Counts, int DaysUntilExam, DateOnly ExamDate)
{
    public RoadmapItem? Find(string? slug) =>
        slug is null ? null : Spine.SelectMany(item => item.Topics.Prepend(item)).FirstOrDefault(item => item.Node.Slug == slug);
}

public sealed class RoadmapService(ApplicationDbContext db, IOptions<ExamOptions> exam, LocalCalendar calendar, TimeProvider clock)
{
    public async Task<RoadmapView> GetAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var nodes = await db.RoadmapNodes.AsNoTracking().ToListAsync(cancellationToken);

        var statuses = userId is null
            ? []
            : await db.RoadmapProgress.AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.NodeId, p => p.Status, cancellationToken);

        NodeStatus? StatusOf(RoadmapNode node) => statuses.TryGetValue(node.Id, out var status) ? status : null;

        var topicsBySubject = nodes.Where(n => n.ParentId is not null).ToLookup(n => n.ParentId!.Value);
        var spine = nodes
            .Where(n => n.ParentId is null)
            .OrderBy(n => n.Order)
            .Select(n => new RoadmapItem(n, StatusOf(n),
                topicsBySubject[n.Id].OrderBy(t => t.Order).Select(t => new RoadmapItem(t, StatusOf(t), [])).ToList()))
            .ToList();

        // O progresso conta matérias e tópicos; checkpoints são marcos e não entram na porcentagem.
        var trackable = nodes.Where(n => n.Kind != NodeKind.Checkpoint).Select(n => StatusOf(n)).ToList();
        var counts = new RoadmapCounts(
            trackable.Count,
            trackable.Count(s => s == NodeStatus.Done),
            trackable.Count(s => s == NodeStatus.InProgress),
            trackable.Count(s => s == NodeStatus.Skipped));

        return new RoadmapView(spine, counts, DaysUntilExam(), exam.Value.ExamDate);
    }

    /// <summary>Grava o status do nó para o aluno; <paramref name="status"/> nulo limpa o status.</summary>
    public async Task<bool> SetStatusAsync(string userId, string slug, NodeStatus? status, CancellationToken cancellationToken = default)
    {
        var node = await db.RoadmapNodes.AsNoTracking().FirstOrDefaultAsync(n => n.Slug == slug, cancellationToken);
        if (node is null)
        {
            return false;
        }

        var progress = await db.RoadmapProgress.FindAsync([userId, node.Id], cancellationToken);
        if (status is null)
        {
            if (progress is not null)
            {
                db.RoadmapProgress.Remove(progress);
            }
        }
        else if (progress is null)
        {
            db.RoadmapProgress.Add(new RoadmapProgress { UserId = userId, NodeId = node.Id, Status = status.Value, UpdatedAt = clock.GetUtcNow() });
        }
        else
        {
            progress.Status = status.Value;
            progress.UpdatedAt = clock.GetUtcNow();
        }

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private int DaysUntilExam() => Math.Max(0, exam.Value.ExamDate.DayNumber - calendar.Today().DayNumber);
}
