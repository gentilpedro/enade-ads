namespace EnadeAds.Blog;

public enum PostCategory
{
    DatesAndDeadlines = 0,
    OrdinancesAndNotices = 1,
    StudyTips = 2,
    ClassAnnouncements = 3,
}

/// <summary>Post do blog de atualizações sobre o ENADE. Sem <see cref="PublishedAt"/> é rascunho.</summary>
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

    public bool IsPublished => PublishedAt is not null && ArchivedAt is null;
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
        _ => category.ToString(),
    };

    /// <summary>Usado na URL do filtro (<c>/blog?categoria=datas-e-prazos</c>).</summary>
    public static string Slug(this PostCategory category) => category switch
    {
        PostCategory.DatesAndDeadlines => "datas-e-prazos",
        PostCategory.OrdinancesAndNotices => "portarias-e-editais",
        PostCategory.StudyTips => "dicas-de-estudo",
        PostCategory.ClassAnnouncements => "avisos-da-turma",
        _ => category.ToString().ToLowerInvariant(),
    };

    public static PostCategory? FromSlug(string? slug) => All.Cast<PostCategory?>().FirstOrDefault(c => c!.Value.Slug() == slug);
}
