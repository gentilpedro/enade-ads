using EnadeAds.Data;
using EnadeAds.Questions;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Blog;

public sealed record PostPage(IReadOnlyList<Post> Items, int Page, int TotalPages);

public sealed record CommentView(PostComment Comment, string AuthorName);

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

public sealed class BlogService(ApplicationDbContext db, TimeProvider clock, ILogger<BlogService> logger)
{
    public const int PageSize = 10;
    public const int TitleMaxLength = 150;
    public const int SummaryMaxLength = 300;
    public const int BodyMaxLength = 20000;
    public const int SourceMaxLength = 500;
    public const int CommentMaxLength = 2000;

    /// <summary>Posts publicados, do mais novo para o mais antigo; o fixado vem antes na primeira página.</summary>
    public async Task<PostPage> ListAsync(PostCategory? category, int page, CancellationToken cancellationToken = default)
    {
        var query = Published();
        if (category is not null)
        {
            query = query.Where(p => p.Category == category);
        }

        var total = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await query
            .OrderByDescending(p => p.PinnedAt != null).ThenByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .ToListAsync(cancellationToken);

        return new PostPage(items, page, totalPages);
    }

    /// <summary>Para a página inicial: o post fixado (se houver) e os mais recentes.</summary>
    public async Task<IReadOnlyList<Post>> HighlightsAsync(int count = 3, CancellationToken cancellationToken = default) =>
        await Published()
            .OrderByDescending(p => p.PinnedAt != null).ThenByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Post>> FeedAsync(int count = 20, CancellationToken cancellationToken = default) =>
        await Published().OrderByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id).Take(count).ToListAsync(cancellationToken);

    /// <summary>Rascunhos e posts arquivados de quem pode editá-los.</summary>
    public async Task<IReadOnlyList<Post>> NotPublishedAsync(Editor editor, CancellationToken cancellationToken = default) =>
        await db.Posts.AsNoTracking()
            .Where(p => p.PublishedAt == null || p.ArchivedAt != null)
            .Where(p => editor.IsAdmin || p.AuthorId == editor.UserId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);

    public Task<Post?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public Task<Post?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<string> AuthorNameAsync(string? authorId, CancellationToken cancellationToken = default)
    {
        var name = authorId is null ? null : await db.Users.Where(u => u.Id == authorId).Select(u => u.FullName).FirstOrDefaultAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(name) ? "Equipe ENADE ADS" : name;
    }

    public static bool CanEdit(Editor? editor, Post post) =>
        editor is not null && (editor.IsAdmin || (editor.IsTeacher && post.AuthorId == editor.UserId));

    /// <summary>Rascunho e arquivado só existem para quem pode editar.</summary>
    public static bool CanView(Editor? editor, Post post) => post.IsPublished || CanEdit(editor, post);

    public async Task<EditResult> CreateAsync(Editor editor, PostDraft draft, PostAction action, CancellationToken cancellationToken = default)
    {
        if (!editor.CanCreate)
        {
            return EditResult.Fail("Só professores e administradores publicam no blog.");
        }

        var errors = Validate(draft);
        if (errors.Count > 0)
        {
            return new EditResult(null, errors);
        }

        var now = clock.GetUtcNow();
        var post = new Post
        {
            Slug = await UniqueSlugAsync(draft.Title, null, cancellationToken),
            Title = draft.Title.Trim(),
            Summary = draft.Summary.Trim(),
            Body = Normalize(draft.Body),
            Category = draft.Category,
            SourceUrl = string.IsNullOrWhiteSpace(draft.SourceUrl) ? null : draft.SourceUrl.Trim(),
            AuthorId = editor.UserId,
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
            return EditResult.Fail("Só o autor do post ou um administrador pode editá-lo.");
        }

        var errors = Validate(draft);
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
        post.Summary = draft.Summary.Trim();
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

    /// <summary>Fixar deixa o post no topo do blog e da página inicial; só um fica fixado por vez.</summary>
    public async Task<EditResult> SetPinnedAsync(Editor editor, int postId, bool pinned, CancellationToken cancellationToken = default)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null || !CanEdit(editor, post))
        {
            return EditResult.Fail("Só o autor do post ou um administrador pode fixá-lo.");
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

    public async Task<IReadOnlyList<CommentView>> CommentsAsync(int postId, CancellationToken cancellationToken = default)
    {
        var rows = await db.PostComments.AsNoTracking()
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.Id)
            .GroupJoin(db.Users, c => c.UserId, u => u.Id, (c, authors) => new { Comment = c, authors })
            .SelectMany(x => x.authors.DefaultIfEmpty(), (x, u) => new { x.Comment, FullName = u == null ? null : u.FullName })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new CommentView(r.Comment, AuthorLabel(r.Comment.UserId, r.FullName))).ToList();
    }

    /// <summary>Comentar exige conta e só vale em post publicado.</summary>
    public async Task<PostComment?> AddCommentAsync(string userId, int postId, string? text, CancellationToken cancellationToken = default)
    {
        var trimmed = CleanComment(text);
        if (trimmed is null || !await db.Posts.AnyAsync(p => p.Id == postId && p.PublishedAt != null && p.ArchivedAt == null, cancellationToken))
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

        var isModerator = editor is not null && (editor.IsAdmin || editor.IsTeacher);
        if (comment.UserId != userId && !isModerator)
        {
            return false;
        }

        db.PostComments.Remove(comment);
        await db.SaveChangesAsync(cancellationToken);
        if (comment.UserId != userId)
        {
            logger.LogInformation("Comentário {CommentId} removido por moderação ({UserId}).", comment.Id, userId);
        }

        return true;
    }

    private static string AuthorLabel(string? userId, string? fullName) =>
        userId is null ? "Participante que excluiu a conta" : string.IsNullOrWhiteSpace(fullName) ? "Participante" : fullName;

    public static bool CanModerate(Editor? editor) => editor is not null && (editor.IsAdmin || editor.IsTeacher);

    private IQueryable<Post> Published() =>
        db.Posts.AsNoTracking().Where(p => p.PublishedAt != null && p.ArchivedAt == null);

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

    private static List<string> Validate(PostDraft draft)
    {
        var errors = new List<string>();
        Require(draft.Title, "o título", TitleMaxLength, errors);
        Require(draft.Summary, "o resumo", SummaryMaxLength, errors);
        Require(draft.Body, "o texto", BodyMaxLength, errors);

        if (!Enum.IsDefined(draft.Category))
        {
            errors.Add("Escolha a categoria.");
        }

        if (!string.IsNullOrWhiteSpace(draft.SourceUrl))
        {
            var source = draft.SourceUrl.Trim();
            if (source.Length > SourceMaxLength || !Uri.TryCreate(source, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            {
                errors.Add("A fonte precisa ser um endereço https completo, por exemplo https://www.gov.br/inep/...");
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

    private static string? CleanComment(string? text)
    {
        var trimmed = text?.Replace("\r\n", "\n").Trim();
        return string.IsNullOrEmpty(trimmed) || trimmed.Length > CommentMaxLength ? null : trimmed;
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();
}
