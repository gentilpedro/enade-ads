using EnadeAds.Blog;
using EnadeAds.Data;
using EnadeAds.Questions;
using EnadeAds.Reviews;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Study;

/// <summary>
/// Edição do roadmap por professores e admins. As matérias (da portaria ou matriz da prova, e a Formação Geral) não
/// podem ser removidas; assuntos e checkpoints podem ser criados, editados, reordenados e removidos.
/// Alteração de professor vai para a fila de revisão (<see cref="ReviewQueue"/>) antes de valer.
/// </summary>
public sealed class RoadmapEditor(ApplicationDbContext db, ReviewQueue reviews, ILogger<RoadmapEditor> logger)
{
    public const int TitleMaxLength = 200;
    public const int SummaryMaxLength = 2000;

    /// <summary>Ids de nós criados pela tela começam aqui, longe dos ids fixos da seed.</summary>
    private const int FirstCustomId = 20_000;

    public static bool CanEdit(Editor? editor) => editor is not null && (editor.IsTeacher || editor.IsAdmin);

    public async Task<EditResult> UpdateAsync(Editor editor, int nodeId, string title, string summary, CancellationToken cancellationToken = default)
    {
        if (!CanEdit(editor))
        {
            return EditResult.Fail("Só professores e administradores editam o roadmap.");
        }

        var errors = Validate(title, summary);
        var node = await db.RoadmapNodes.FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken);
        if (node is null)
        {
            return EditResult.Fail("Tópico não encontrado.");
        }

        if (errors.Count > 0)
        {
            return new EditResult(nodeId, errors);
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, ReviewKind.RoadmapUpdate, nodeId, new RoadmapTextPayload(title, summary),
                $"{node.Title} → {title}", cancellationToken);
        }

        // O endereço (slug) não muda: links compartilhados para o tópico continuam valendo.
        node.Title = title.Trim();
        node.Summary = summary.Trim();
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Nó do roadmap {NodeId} editado por {UserId}.", nodeId, editor.UserId);
        return new EditResult(nodeId, []);
    }

    public async Task<EditResult> AddTopicAsync(Editor editor, int subjectId, string title, string summary, CancellationToken cancellationToken = default)
    {
        if (!CanEdit(editor))
        {
            return EditResult.Fail("Só professores e administradores editam o roadmap.");
        }

        var subject = await db.RoadmapNodes.FirstOrDefaultAsync(n => n.Id == subjectId && n.Kind == NodeKind.Subject, cancellationToken);
        if (subject is null)
        {
            return EditResult.Fail("Matéria não encontrada.");
        }

        var errors = Validate(title, summary);
        if (errors.Count > 0)
        {
            return new EditResult(null, errors);
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, ReviewKind.RoadmapAddTopic, subjectId, new RoadmapTextPayload(title, summary),
                $"{subject.Title}: {title}", cancellationToken);
        }

        var lastOrder = await db.RoadmapNodes.Where(n => n.ParentId == subjectId).MaxAsync(n => (int?)n.Order, cancellationToken) ?? 0;
        var topic = new RoadmapNode
        {
            Id = await NextIdAsync(cancellationToken),
            TrackId = subject.TrackId,
            Slug = await UniqueSlugAsync(title, cancellationToken),
            Title = title.Trim(),
            Summary = summary.Trim(),
            Kind = NodeKind.Topic,
            Block = subject.Block,
            Order = lastOrder + 1,
            ParentId = subject.Id,
        };

        db.RoadmapNodes.Add(topic);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Assunto {NodeId} criado em {SubjectId} por {UserId}.", topic.Id, subjectId, editor.UserId);
        return new EditResult(topic.Id, []);
    }

    /// <summary>Novo checkpoint na espinha, logo depois da matéria indicada.</summary>
    public async Task<EditResult> AddCheckpointAfterAsync(Editor editor, int subjectId, string title, string summary, CancellationToken cancellationToken = default)
    {
        if (!CanEdit(editor))
        {
            return EditResult.Fail("Só professores e administradores editam o roadmap.");
        }

        var subject = await db.RoadmapNodes.FirstOrDefaultAsync(n => n.Id == subjectId && n.Kind == NodeKind.Subject, cancellationToken);
        if (subject is null)
        {
            return EditResult.Fail("Matéria não encontrada.");
        }

        var errors = Validate(title, summary);
        if (errors.Count > 0)
        {
            return new EditResult(null, errors);
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, ReviewKind.RoadmapAddCheckpoint, subjectId, new RoadmapTextPayload(title, summary),
                $"Depois de {subject.Title}: {title}", cancellationToken);
        }

        // Abre espaço na espinha: tudo depois da matéria anda uma posição.
        var after = await db.RoadmapNodes.Where(n => n.ParentId == null && n.Order > subject.Order).ToListAsync(cancellationToken);
        foreach (var node in after)
        {
            node.Order++;
        }

        var checkpoint = new RoadmapNode
        {
            Id = await NextIdAsync(cancellationToken),
            TrackId = subject.TrackId,
            Slug = await UniqueSlugAsync(title, cancellationToken),
            Title = title.Trim(),
            Summary = summary.Trim(),
            Kind = NodeKind.Checkpoint,
            Block = subject.Block,
            Order = subject.Order + 1,
        };

        db.RoadmapNodes.Add(checkpoint);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Checkpoint {NodeId} criado depois de {SubjectId} por {UserId}.", checkpoint.Id, subjectId, editor.UserId);
        return new EditResult(checkpoint.Id, []);
    }

    /// <summary>
    /// Troca o nó de lugar com o vizinho: assuntos dentro da mesma matéria; matérias e checkpoints dentro do mesmo bloco,
    /// para os blocos da espinha não se misturarem.
    /// </summary>
    public async Task<EditResult> MoveAsync(Editor editor, int nodeId, bool up, CancellationToken cancellationToken = default)
    {
        if (!CanEdit(editor))
        {
            return EditResult.Fail("Só professores e administradores editam o roadmap.");
        }

        var node = await db.RoadmapNodes.FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken);
        if (node is null)
        {
            return EditResult.Fail("Tópico não encontrado.");
        }

        var siblings = db.RoadmapNodes.Where(n => n.ParentId == node.ParentId);
        if (node.ParentId is null)
        {
            siblings = siblings.Where(n => n.Block == node.Block);
        }

        var neighbor = up
            ? await siblings.Where(n => n.Order < node.Order).OrderByDescending(n => n.Order).FirstOrDefaultAsync(cancellationToken)
            : await siblings.Where(n => n.Order > node.Order).OrderBy(n => n.Order).FirstOrDefaultAsync(cancellationToken);
        if (neighbor is null)
        {
            return EditResult.Fail(up ? "Já é o primeiro." : "Já é o último.");
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, ReviewKind.RoadmapMove, nodeId, new RoadmapMovePayload(up),
                $"{node.Title}: {(up ? "subir" : "descer")} uma posição", cancellationToken);
        }

        (node.Order, neighbor.Order) = (neighbor.Order, node.Order);
        await db.SaveChangesAsync(cancellationToken);
        return new EditResult(nodeId, []);
    }

    /// <summary>Remove um assunto ou checkpoint. Apaga só o progresso dos alunos nesse nó.</summary>
    public async Task<EditResult> RemoveAsync(Editor editor, int nodeId, CancellationToken cancellationToken = default)
    {
        if (!CanEdit(editor))
        {
            return EditResult.Fail("Só professores e administradores editam o roadmap.");
        }

        var node = await db.RoadmapNodes.FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken);
        if (node is null)
        {
            return EditResult.Fail("Tópico não encontrado.");
        }

        if (node.Kind == NodeKind.Subject)
        {
            return EditResult.Fail("As matérias vêm da matriz oficial da prova e não podem ser removidas. Edite o resumo ou os assuntos.");
        }

        if (editor.NeedsReview)
        {
            return await reviews.SubmitAsync(editor, ReviewKind.RoadmapRemove, nodeId, null, node.Title, cancellationToken);
        }

        db.RoadmapNodes.Remove(node);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Nó do roadmap {NodeId} ({Slug}) removido por {UserId}.", node.Id, node.Slug, editor.UserId);
        return new EditResult(node.ParentId, []);
    }

    private async Task<int> NextIdAsync(CancellationToken cancellationToken)
    {
        // O id é único entre todas as trilhas, então olha também os nós das outras.
        var max = await db.RoadmapNodes.IgnoreQueryFilters().MaxAsync(n => (int?)n.Id, cancellationToken) ?? 0;
        return Math.Max(max + 1, FirstCustomId);
    }

    private async Task<string> UniqueSlugAsync(string title, CancellationToken cancellationToken)
    {
        var baseSlug = Slugs.From(title);
        var slug = baseSlug;
        for (var suffix = 2; await db.RoadmapNodes.AnyAsync(n => n.Slug == slug, cancellationToken); suffix++)
        {
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    private static List<string> Validate(string? title, string? summary)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add("Preencha o título.");
        }
        else if (title.Trim().Length > TitleMaxLength)
        {
            errors.Add($"Encurte o título: o limite é {TitleMaxLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            errors.Add("Preencha a explicação.");
        }
        else if (summary.Trim().Length > SummaryMaxLength)
        {
            errors.Add($"Encurte a explicação: o limite é {SummaryMaxLength} caracteres.");
        }

        return errors;
    }
}
