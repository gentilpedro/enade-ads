using System.Text.RegularExpressions;
using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Blog;

public enum FeedSort
{
    Hot,
    New,
}

public sealed record FeedQuery(FeedSort Sort, PostCategory? Category, bool OfficialOnly, int Page);

public sealed record FeedItem(Post Post, string AuthorName, int CommentCount, short MyVote);

public sealed record FeedPage(IReadOnlyList<FeedItem> Items, int Page, int TotalPages);

public sealed record CommentView(PostComment Comment, string AuthorName);

/// <summary>Denúncias abertas de um mesmo post ou comentário, agrupadas para a moderação decidir uma vez só.</summary>
public sealed record ReportGroup(
    int PostId, long? CommentId, string PostTitle, string PostSlug, string Content, string AuthorName,
    int Count, IReadOnlyList<string> Reasons, DateTimeOffset FirstReportedAt);

public sealed class PostDraft
{
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Body { get; set; } = "";
    public PostCategory Category { get; set; }
    public string SourceUrl { get; set; } = "";
}

public enum PostAction
{
    SaveDraft,
    Publish,
}

public sealed partial class BlogService(ApplicationDbContext db, TimeProvider clock, ILogger<BlogService> logger)
{
    public const int PageSize = 20;
    public const int TitleMaxLength = 150;
    public const int SummaryMaxLength = 300;
    public const int BodyMaxLength = 20000;
    public const int SourceMaxLength = 500;
    public const int CommentMaxLength = 2000;
    public const int ReportReasonMaxLength = 500;
    public const int ExcerptLength = 220;

    /// <summary>Limite de posts por hora para alunos, contra spam. Professor e admin não têm limite.</summary>
    public const int StudentPostsPerHour = 5;

    /// <summary>
    /// Feed da página inicial. O fixado vem sempre primeiro. "Em alta" pondera a pontuação pela idade do post
    /// (a mesma ideia do Hacker News), para um post bem votado subir sem ficar no topo para sempre.
    /// </summary>
    public async Task<FeedPage> ListAsync(FeedQuery feed, string? userId, CancellationToken cancellationToken = default)
    {
        var query = Published();
        if (feed.Category is not null)
        {
            query = query.Where(p => p.Category == feed.Category);
        }

        if (feed.OfficialOnly)
        {
            query = query.Where(p => p.IsOfficial);
        }

        // Ordena em memória só com os campos de ordenação: a fórmula de "Em alta" não vira SQL portável,
        // e o volume (posts de uma turma) é pequeno. Os posts completos vêm depois, só os da página.
        var rows = await query.Select(p => new { p.Id, p.Score, PublishedAt = p.PublishedAt!.Value, Pinned = p.PinnedAt != null }).ToListAsync(cancellationToken);
        var now = clock.GetUtcNow();
        var ordered = feed.Sort == FeedSort.New
            ? rows.OrderByDescending(r => r.Pinned).ThenByDescending(r => r.PublishedAt).ThenByDescending(r => r.Id)
            : rows.OrderByDescending(r => r.Pinned).ThenByDescending(r => Hotness(r.Score, r.PublishedAt, now)).ThenByDescending(r => r.Id);

        var totalPages = Math.Max(1, (int)Math.Ceiling(rows.Count / (double)PageSize));
        var page = Math.Clamp(feed.Page, 1, totalPages);
        var ids = ordered.Skip((page - 1) * PageSize).Take(PageSize).Select(r => r.Id).ToList();

        return new FeedPage(await ItemsAsync(ids, userId, cancellationToken), page, totalPages);
    }

    /// <summary>Pontos divididos pela idade em horas: um voto hoje vale mais que um voto na semana passada.</summary>
    public static double Hotness(int score, DateTimeOffset publishedAt, DateTimeOffset now)
    {
        var hours = Math.Max(0, (now - publishedAt).TotalHours);
        return (score + 1) / Math.Pow(hours + 2, 1.5);
    }

    /// <summary>RSS: só os avisos oficiais, que é o que faz sentido acompanhar de fora.</summary>
    public async Task<IReadOnlyList<Post>> FeedAsync(int count = 20, CancellationToken cancellationToken = default) =>
        await Published().Where(p => p.IsOfficial)
            .OrderByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id).Take(count).ToListAsync(cancellationToken);

    /// <summary>Rascunhos, arquivados e removidos do próprio autor.</summary>
    public async Task<IReadOnlyList<Post>> NotPublishedAsync(Editor editor, CancellationToken cancellationToken = default) =>
        await db.Posts.AsNoTracking()
            .Where(p => p.PublishedAt == null || p.ArchivedAt != null || p.RemovedAt != null)
            .Where(p => p.AuthorId == editor.UserId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);

    public Task<Post?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public Task<Post?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<string> AuthorNameAsync(Post post, CancellationToken cancellationToken = default)
    {
        var name = post.AuthorId is null ? null : await db.Users.Where(u => u.Id == post.AuthorId).Select(u => u.FullName).FirstOrDefaultAsync(cancellationToken);
        return PostAuthorLabel(post, name);
    }

    public async Task<short> MyVoteAsync(string? userId, int postId, CancellationToken cancellationToken = default) =>
        userId is null ? (short)0 : await db.PostVotes.Where(v => v.PostId == postId && v.UserId == userId).Select(v => v.Value).FirstOrDefaultAsync(cancellationToken);

    /// <summary>O autor edita o próprio post; admin edita qualquer um. Post removido pela moderação fica travado para o autor.</summary>
    public static bool CanEdit(Editor? editor, Post post) =>
        editor is not null && (editor.IsAdmin || (post.AuthorId == editor.UserId && post.RemovedAt is null));

    /// <summary>Fixar é coisa de aviso oficial: professor nos próprios posts, admin em qualquer um.</summary>
    public static bool CanPin(Editor? editor, Post post) => editor is not null && editor.CanCreate && CanEdit(editor, post);

    public static bool CanModerate(Editor? editor) => editor is not null && (editor.IsAdmin || editor.IsTeacher);

    /// <summary>Publicado: todo mundo. Rascunho e arquivado: o autor. Removido: o autor (para saber o que houve) e a moderação.</summary>
    public static bool CanView(Editor? editor, Post post) =>
        post.IsPublished
        || (editor is not null && (editor.IsAdmin || post.AuthorId == editor.UserId))
        || (post.RemovedAt is not null && CanModerate(editor));

    public async Task<EditResult> CreateAsync(Editor editor, PostDraft draft, PostAction action, CancellationToken cancellationToken = default)
    {
        var errors = Validate(editor.CanCreate, draft);
        if (errors.Count > 0)
        {
            return new EditResult(null, errors);
        }

        var now = clock.GetUtcNow();
        if (!editor.CanCreate)
        {
            var since = now.AddHours(-1);
            var recent = await db.Posts.CountAsync(p => p.AuthorId == editor.UserId && p.CreatedAt > since, cancellationToken);
            if (recent >= StudentPostsPerHour)
            {
                return EditResult.Fail($"Você já criou {StudentPostsPerHour} posts na última hora. Espere um pouco antes de publicar de novo.");
            }
        }

        var post = new Post
        {
            Slug = await UniqueSlugAsync(draft.Title, null, cancellationToken),
            Title = draft.Title.Trim(),
            Summary = SummaryOrExcerpt(draft),
            Body = Normalize(draft.Body),
            Category = draft.Category,
            SourceUrl = string.IsNullOrWhiteSpace(draft.SourceUrl) ? null : draft.SourceUrl.Trim(),
            AuthorId = editor.UserId,
            IsOfficial = editor.CanCreate,
            CreatedAt = now,
            UpdatedAt = now,
            PublishedAt = action == PostAction.Publish ? now : null,
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Post {PostId} criado por {UserId} ({Action}).", post.Id, editor.UserId, action);
        return new EditResult(post.Id, []);
    }

    public async Task<EditResult> UpdateAsync(Editor editor, int postId, PostDraft draft, PostAction action, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null)
        {
            return EditResult.Fail("Post não encontrado.");
        }

        if (!CanEdit(editor, post))
        {
            return EditResult.Fail(post.RemovedAt is null
                ? "Só o autor do post ou um administrador pode editá-lo."
                : "Este post foi removido pela moderação e não pode ser editado.");
        }

        // Quem decide se a categoria oficial vale é quem está editando: o aluno não move o próprio post para lá.
        var errors = Validate(editor.CanCreate, draft);
        if (errors.Count > 0)
        {
            return new EditResult(postId, errors);
        }

        // O endereço só muda enquanto o post é rascunho: depois de publicado, links compartilhados continuam valendo.
        if (post.PublishedAt is null && draft.Title.Trim() != post.Title)
        {
            post.Slug = await UniqueSlugAsync(draft.Title, post.Id, cancellationToken);
        }

        post.Title = draft.Title.Trim();
        post.Summary = SummaryOrExcerpt(draft);
        post.Body = Normalize(draft.Body);
        post.Category = draft.Category;
        post.SourceUrl = string.IsNullOrWhiteSpace(draft.SourceUrl) ? null : draft.SourceUrl.Trim();
        post.UpdatedAt = clock.GetUtcNow();
        if (action == PostAction.Publish)
        {
            post.PublishedAt ??= post.UpdatedAt;
            post.ArchivedAt = null;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Post {PostId} editado por {UserId} ({Action}).", post.Id, editor.UserId, action);
        return new EditResult(post.Id, []);
    }

    /// <summary>Fixar deixa o post no topo do feed; só um fica fixado por vez.</summary>
    public async Task<EditResult> SetPinnedAsync(Editor editor, int postId, bool pinned, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null || !CanPin(editor, post))
        {
            return EditResult.Fail("Só o professor autor do post ou um administrador pode fixá-lo.");
        }

        if (pinned && !post.IsPublished)
        {
            return EditResult.Fail("Publique o post antes de fixá-lo.");
        }

        if (pinned)
        {
            await db.Posts.Where(p => p.PinnedAt != null && p.Id != postId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.PinnedAt, (DateTimeOffset?)null), cancellationToken);
        }

        post.PinnedAt = pinned ? clock.GetUtcNow() : null;
        await db.SaveChangesAsync(cancellationToken);
        return new EditResult(post.Id, []);
    }

    public async Task<EditResult> ArchiveAsync(Editor editor, int postId, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null || !CanEdit(editor, post))
        {
            return EditResult.Fail("Só o autor do post ou um administrador pode arquivá-lo.");
        }

        post.ArchivedAt ??= clock.GetUtcNow();
        post.PinnedAt = null;
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Post {PostId} arquivado por {UserId}.", post.Id, editor.UserId);
        return new EditResult(post.Id, []);
    }

    /// <summary>Moderação: tira o post do feed e fecha as denúncias dele. Diferente de arquivar, o autor não desfaz.</summary>
    public async Task<bool> RemoveAsync(Editor? editor, int postId, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null || !CanModerate(editor))
        {
            return false;
        }

        var now = clock.GetUtcNow();
        post.RemovedAt ??= now;
        post.RemovedById ??= editor!.UserId;
        post.PinnedAt = null;
        await ResolveReportsAsync(editor!, postId, null, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Post {PostId} removido por moderação ({UserId}).", post.Id, editor!.UserId);
        return true;
    }

    public async Task<bool> RestoreAsync(Editor? editor, int postId, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null || post.RemovedAt is null || !CanModerate(editor))
        {
            return false;
        }

        post.RemovedAt = null;
        post.RemovedById = null;
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Post {PostId} restaurado por {UserId}.", post.Id, editor!.UserId);
        return true;
    }

    /// <summary>+1 ou -1 em post publicado. Repetir o mesmo voto desfaz; o voto contrário troca.</summary>
    public async Task<bool> VoteAsync(string userId, int postId, short value, CancellationToken cancellationToken = default)
    {
        if (value is not (1 or -1) || !await Published().AnyAsync(p => p.Id == postId, cancellationToken))
        {
            return false;
        }

        var vote = await db.PostVotes.FirstOrDefaultAsync(v => v.PostId == postId && v.UserId == userId, cancellationToken);
        if (vote is null)
        {
            db.PostVotes.Add(new PostVote { PostId = postId, UserId = userId, Value = value, CreatedAt = clock.GetUtcNow() });
        }
        else if (vote.Value == value)
        {
            db.PostVotes.Remove(vote);
        }
        else
        {
            vote.Value = value;
        }

        await db.SaveChangesAsync(cancellationToken);

        // Recalcula a soma no banco em vez de somar +1/-1 no objeto: dois votos ao mesmo tempo não deixam a pontuação errada.
        await db.Posts.Where(p => p.Id == postId).ExecuteUpdateAsync(
            set => set.SetProperty(p => p.Score, p => db.PostVotes.Where(v => v.PostId == p.Id).Sum(v => (int)v.Value)), cancellationToken);
        return true;
    }

    /// <summary>Qualquer pessoa logada denuncia um post ou comentário. Denunciar de novo o mesmo item não duplica.</summary>
    public async Task<bool> ReportAsync(string userId, int postId, long? commentId, string? reason, CancellationToken cancellationToken = default)
    {
        var text = reason?.Trim();
        if (text is { Length: > ReportReasonMaxLength })
        {
            return false;
        }

        var exists = commentId is null
            ? await Published().AnyAsync(p => p.Id == postId, cancellationToken)
            : await db.PostComments.AnyAsync(c => c.Id == commentId && c.PostId == postId, cancellationToken);
        if (!exists)
        {
            return false;
        }

        var already = await db.PostReports.AnyAsync(r => r.ReporterId == userId && r.PostId == postId && r.CommentId == commentId && r.ResolvedAt == null, cancellationToken);
        if (!already)
        {
            db.PostReports.Add(new PostReport
            {
                PostId = postId,
                CommentId = commentId,
                ReporterId = userId,
                Reason = string.IsNullOrEmpty(text) ? null : text,
                CreatedAt = clock.GetUtcNow(),
            });
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Denúncia do post {PostId} (comentário {CommentId}) por {UserId}.", postId, commentId, userId);
        }

        return true;
    }

    public Task<int> OpenReportCountAsync(CancellationToken cancellationToken = default) =>
        db.PostReports.Where(r => r.ResolvedAt == null).Select(r => new { r.PostId, r.CommentId }).Distinct().CountAsync(cancellationToken);

    public async Task<IReadOnlyList<ReportGroup>> OpenReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await db.PostReports.AsNoTracking().Where(r => r.ResolvedAt == null).OrderBy(r => r.Id).ToListAsync(cancellationToken);
        var postIds = reports.Select(r => r.PostId).Distinct().ToList();
        var commentIds = reports.Where(r => r.CommentId != null).Select(r => r.CommentId!.Value).Distinct().ToList();

        var posts = await db.Posts.AsNoTracking().Where(p => postIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);
        var comments = await db.PostComments.AsNoTracking().Where(c => commentIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, cancellationToken);
        var authorIds = posts.Values.Select(p => p.AuthorId).Concat(comments.Values.Select(c => c.UserId)).OfType<string>().Distinct().ToList();
        var names = await db.Users.Where(u => authorIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        return reports
            .GroupBy(r => (r.PostId, r.CommentId))
            .Where(g => posts.ContainsKey(g.Key.PostId))
            .Select(g =>
            {
                var post = posts[g.Key.PostId];
                var comment = g.Key.CommentId is { } id ? comments.GetValueOrDefault(id) : null;
                var author = comment is null
                    ? PostAuthorLabel(post, post.AuthorId is null ? null : names.GetValueOrDefault(post.AuthorId))
                    : CommentAuthorLabel(comment.UserId, comment.UserId is null ? null : names.GetValueOrDefault(comment.UserId));
                return new ReportGroup(post.Id, g.Key.CommentId, post.Title, post.Slug, comment?.Text ?? Excerpt(post.Body), author,
                    g.Count(), g.Select(r => r.Reason).OfType<string>().ToList(), g.Min(r => r.CreatedAt));
            })
            .OrderBy(g => g.FirstReportedAt)
            .ToList();
    }

    /// <summary>Encerra as denúncias de um item: removendo o conteúdo ou mantendo (denúncia improcedente).</summary>
    public async Task<bool> ResolveAsync(Editor? editor, int postId, long? commentId, bool removeContent, CancellationToken cancellationToken = default)
    {
        if (!CanModerate(editor))
        {
            return false;
        }

        if (removeContent)
        {
            return commentId is null
                ? await RemoveAsync(editor, postId, cancellationToken)
                : await DeleteCommentAsync(editor, editor!.UserId, commentId.Value, cancellationToken);
        }

        await ResolveReportsAsync(editor!, postId, commentId, clock.GetUtcNow(), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CommentView>> CommentsAsync(int postId, CancellationToken cancellationToken = default)
    {
        var rows = await db.PostComments.AsNoTracking()
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.Id)
            .GroupJoin(db.Users, c => c.UserId, u => u.Id, (c, authors) => new { Comment = c, authors })
            .SelectMany(x => x.authors.DefaultIfEmpty(), (x, u) => new { x.Comment, FullName = u == null ? null : u.FullName })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new CommentView(r.Comment, CommentAuthorLabel(r.Comment.UserId, r.FullName))).ToList();
    }

    /// <summary>Comentar exige conta e só vale em post publicado.</summary>
    public async Task<PostComment?> AddCommentAsync(string userId, int postId, string? text, CancellationToken cancellationToken = default)
    {
        var trimmed = CleanComment(text);
        if (trimmed is null || !await Published().AnyAsync(p => p.Id == postId, cancellationToken))
        {
            return null;
        }

        var comment = new PostComment { PostId = postId, UserId = userId, Text = trimmed, CreatedAt = clock.GetUtcNow() };
        db.PostComments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);
        return comment;
    }

    /// <summary>Só o autor do comentário edita o texto.</summary>
    public async Task<bool> EditCommentAsync(string userId, long commentId, string? text, CancellationToken cancellationToken = default)
    {
        var trimmed = CleanComment(text);
        var comment = await db.PostComments.FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
        if (trimmed is null || comment is null || comment.UserId != userId)
        {
            return false;
        }

        comment.Text = trimmed;
        comment.EditedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>O autor exclui o próprio comentário; professor e admin excluem qualquer um (moderação).</summary>
    public async Task<bool> DeleteCommentAsync(Editor? editor, string userId, long commentId, CancellationToken cancellationToken = default)
    {
        var comment = await db.PostComments.FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
        if (comment is null)
        {
            return false;
        }

        if (comment.UserId != userId && !CanModerate(editor))
        {
            return false;
        }

        // As denúncias do comentário saem junto (cascata); o log guarda quem removeu.
        db.PostComments.Remove(comment);
        await db.SaveChangesAsync(cancellationToken);
        if (comment.UserId != userId)
        {
            logger.LogInformation("Comentário {CommentId} removido por moderação ({UserId}).", comment.Id, userId);
        }

        return true;
    }

    /// <summary>Começo do texto sem a marcação, para o feed quando o autor não escreveu resumo.</summary>
    public static string Excerpt(string body, int length = ExcerptLength)
    {
        var text = ImageMarkup().Replace(body, " ");
        text = LinkMarkup().Replace(text, "$1");
        text = text.Replace("```", " ").Replace("**", "").Replace("`", "");
        text = LineStartMarkup().Replace(text, " ");
        text = Whitespace().Replace(text, " ").Trim();
        return text.Length <= length ? text : text[..length].TrimEnd() + "…";
    }

    private async Task<IReadOnlyList<FeedItem>> ItemsAsync(List<int> ids, string? userId, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var posts = await db.Posts.AsNoTracking().Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);
        var comments = await db.PostComments.Where(c => ids.Contains(c.PostId)).GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.PostId, x => x.Count, cancellationToken);
        var authorIds = posts.Values.Select(p => p.AuthorId).OfType<string>().Distinct().ToList();
        var names = await db.Users.Where(u => authorIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);
        var votes = userId is null
            ? []
            : await db.PostVotes.Where(v => v.UserId == userId && ids.Contains(v.PostId)).ToDictionaryAsync(v => v.PostId, v => v.Value, cancellationToken);

        return ids.Select(id =>
        {
            var post = posts[id];
            var name = post.AuthorId is null ? null : names.GetValueOrDefault(post.AuthorId);
            return new FeedItem(post, PostAuthorLabel(post, name), comments.GetValueOrDefault(id), votes.GetValueOrDefault(id));
        }).ToList();
    }

    private async Task ResolveReportsAsync(Editor editor, int postId, long? commentId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        // Remover o post encerra também as denúncias dos comentários dele: sem o post, não há o que moderar.
        var open = await db.PostReports
            .Where(r => r.PostId == postId && r.ResolvedAt == null && (commentId == null ? true : r.CommentId == commentId))
            .ToListAsync(cancellationToken);
        foreach (var report in open)
        {
            report.ResolvedAt = now;
            report.ResolvedById = editor.UserId;
        }
    }

    private static string PostAuthorLabel(Post post, string? fullName) =>
        post.AuthorId is null
            ? post.IsOfficial ? "Equipe ENADE ADS" : "Participante que excluiu a conta"
            : string.IsNullOrWhiteSpace(fullName) ? "Participante" : fullName;

    private static string CommentAuthorLabel(string? userId, string? fullName) =>
        userId is null ? "Participante que excluiu a conta" : string.IsNullOrWhiteSpace(fullName) ? "Participante" : fullName;

    private IQueryable<Post> Published() =>
        db.Posts.AsNoTracking().Where(p => p.PublishedAt != null && p.ArchivedAt == null && p.RemovedAt == null);

    private async Task<string> UniqueSlugAsync(string title, int? ignoreId, CancellationToken cancellationToken)
    {
        var baseSlug = Slugs.From(title);
        var slug = Slugs.IsReserved(baseSlug) ? $"{baseSlug}-2" : baseSlug;
        for (var suffix = 2; await db.Posts.AnyAsync(p => p.Slug == slug && p.Id != ignoreId, cancellationToken); suffix++)
        {
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    private static List<string> Validate(bool official, PostDraft draft)
    {
        var errors = new List<string>();
        Require(draft.Title, "o título", TitleMaxLength, errors);
        Require(draft.Body, "o texto", BodyMaxLength, errors);

        if (draft.Summary?.Trim().Length > SummaryMaxLength)
        {
            errors.Add($"Encurte o resumo: o limite é {SummaryMaxLength} caracteres.");
        }

        if (!Enum.IsDefined(draft.Category))
        {
            errors.Add("Escolha a categoria.");
        }
        else if (draft.Category.IsOfficialOnly() && !official)
        {
            errors.Add($"A categoria \"{draft.Category.Name()}\" é para avisos de professores. Escolha Dúvidas, Discussão ou Dicas de estudo.");
        }

        if (!string.IsNullOrWhiteSpace(draft.SourceUrl))
        {
            var source = draft.SourceUrl.Trim();
            if (source.Length > SourceMaxLength || !Uri.TryCreate(source, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            {
                errors.Add("O link precisa ser um endereço https completo, por exemplo https://www.gov.br/inep/...");
            }
        }

        return errors;
    }

    private static void Require(string? value, string field, int maxLength, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"Preencha {field}.");
        }
        else if (value.Trim().Length > maxLength)
        {
            errors.Add($"Encurte {field}: o limite é {maxLength} caracteres.");
        }
    }

    private static string SummaryOrExcerpt(PostDraft draft) =>
        string.IsNullOrWhiteSpace(draft.Summary) ? Excerpt(Normalize(draft.Body)) : draft.Summary.Trim();

    private static string? CleanComment(string? text)
    {
        var trimmed = text?.Replace("\r\n", "\n").Trim();
        return string.IsNullOrEmpty(trimmed) || trimmed.Length > CommentMaxLength ? null : trimmed;
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();

    [GeneratedRegex(@"!\[[^\]]*\]\([^)]*\)")]
    private static partial Regex ImageMarkup();

    [GeneratedRegex(@"\[([^\]]*)\]\([^)]*\)")]
    private static partial Regex LinkMarkup();

    [GeneratedRegex(@"(?m)^\s*-\s+")]
    private static partial Regex LineStartMarkup();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
