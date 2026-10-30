namespace EnadeAds.Blog;

public enum PostCategory
{
    DatesAndDeadlines = 0,
    OrdinancesAndNotices = 1,
    StudyTips = 2,
    ClassAnnouncements = 3,
    Doubts = 4,
    Discussion = 5,
}

/// <summary>Post do feed da comunidade. Sem <see cref="PublishedAt"/> é rascunho.</summary>
public class Post
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    /// <summary>Texto com a marcação mínima do <see cref="Questions.StatementFormatter"/>.</summary>
    public required string Body { get; set; }

    public PostCategory Category { get; set; }

    /// <summary>Fonte oficial (portaria, edital, página do Inep), sempre https.</summary>
    public string? SourceUrl { get; set; }

    public string? AuthorId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? PinnedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>Escrito por professor ou admin: leva o selo "Professor" e entra em "Avisos oficiais" e no RSS.</summary>
    public bool IsOfficial { get; set; }

    /// <summary>Soma dos votos (+1/-1), mantida a cada voto para ordenar o feed sem somar tudo de novo.</summary>
    public int Score { get; set; }

    /// <summary>Removido pela moderação: some do feed e o autor não consegue publicar de novo.</summary>
    public DateTimeOffset? RemovedAt { get; set; }
    public string? RemovedById { get; set; }

    public bool IsPublished => PublishedAt is not null && ArchivedAt is null && RemovedAt is null;
}

/// <summary>Um voto por pessoa por post. Clicar de novo no mesmo voto desfaz.</summary>
public class PostVote
{
    public int PostId { get; set; }
    public required string UserId { get; set; }
    public short Value { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Denúncia de um post ou de um comentário (<see cref="CommentId"/> preenchido), resolvida pela moderação.</summary>
public class PostReport
{
    public long Id { get; set; }
    public int PostId { get; set; }
    public long? CommentId { get; set; }

    /// <summary>Nulo quando quem denunciou excluiu a conta.</summary>
    public string? ReporterId { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public string? ResolvedById { get; set; }
}

public class PostComment
{
    public long Id { get; set; }
    public int PostId { get; set; }

    /// <summary>Nulo quando quem comentou excluiu a conta: o comentário fica, sem o nome (LGPD, #18).</summary>
    public string? UserId { get; set; }
    public required string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
}

public static class PostCategoryNames
{
    public static IReadOnlyList<PostCategory> All { get; } = Enum.GetValues<PostCategory>();

    public static string Name(this PostCategory category) => category switch
    {
        PostCategory.DatesAndDeadlines => "Datas e prazos",
        PostCategory.OrdinancesAndNotices => "Portarias e editais",
        PostCategory.StudyTips => "Dicas de estudo",
        PostCategory.ClassAnnouncements => "Avisos da turma",
        PostCategory.Doubts => "Dúvidas",
        PostCategory.Discussion => "Discussão",
        _ => category.ToString(),
    };

    /// <summary>Usado na URL do filtro (<c>/blog?categoria=datas-e-prazos</c>).</summary>
    public static string Slug(this PostCategory category) => category switch
    {
        PostCategory.DatesAndDeadlines => "datas-e-prazos",
        PostCategory.OrdinancesAndNotices => "portarias-e-editais",
        PostCategory.StudyTips => "dicas-de-estudo",
        PostCategory.ClassAnnouncements => "avisos-da-turma",
        PostCategory.Doubts => "duvidas",
        PostCategory.Discussion => "discussao",
        _ => category.ToString().ToLowerInvariant(),
    };

    /// <summary>Categorias de aviso oficial: só professor e admin publicam nelas.</summary>
    public static bool IsOfficialOnly(this PostCategory category) =>
        category is PostCategory.DatesAndDeadlines or PostCategory.OrdinancesAndNotices or PostCategory.ClassAnnouncements;

    public static IReadOnlyList<PostCategory> AvailableTo(bool official) =>
        official ? All : All.Where(c => !c.IsOfficialOnly()).ToList();

    public static PostCategory? FromSlug(string? slug) => All.Cast<PostCategory?>().FirstOrDefault(c => c!.Value.Slug() == slug);
}
