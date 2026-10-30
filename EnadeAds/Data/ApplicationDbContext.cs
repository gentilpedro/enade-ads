using EnadeAds.Blog;
using EnadeAds.Challenges;
using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EnadeAds.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RoadmapNode> RoadmapNodes => Set<RoadmapNode>();
    public DbSet<RoadmapProgress> RoadmapProgress => Set<RoadmapProgress>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<QuestionAttempt> QuestionAttempts => Set<QuestionAttempt>();
    public DbSet<DiscursiveAnswer> DiscursiveAnswers => Set<DiscursiveAnswer>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeQuestion> ChallengeQuestions => Set<ChallengeQuestion>();
    public DbSet<ChallengeSubmission> ChallengeSubmissions => Set<ChallengeSubmission>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostComment> PostComments => Set<PostComment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RoadmapNode>(node =>
        {
            node.Property(n => n.Id).ValueGeneratedNever();
            node.Property(n => n.Slug).HasMaxLength(80);
            node.Property(n => n.Title).HasMaxLength(200);
            node.Property(n => n.Summary).HasMaxLength(2000);
            node.HasIndex(n => n.Slug).IsUnique();
            node.HasOne(n => n.Parent)
                .WithMany(n => n.Topics)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            node.HasData(RoadmapSeed.Nodes.Select(n => new RoadmapNode
            {
                Id = n.Id, Slug = n.Slug, Title = n.Title, Summary = n.Summary,
                Kind = n.Kind, Block = n.Block, Order = n.Order, ParentId = n.ParentId,
            }));
        });

        builder.Entity<RoadmapProgress>(progress =>
        {
            progress.HasKey(p => new { p.UserId, p.NodeId });
            progress.HasOne<ApplicationUser>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            progress.HasOne<RoadmapNode>().WithMany().HasForeignKey(p => p.NodeId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Question>(question =>
        {
            question.Property(q => q.Statement).HasMaxLength(8000);
            question.Property(q => q.Source).HasMaxLength(200);
            question.Property(q => q.ExpectedAnswer).HasMaxLength(QuestionEditor.ExpectedAnswerMaxLength);
            question.HasOne(q => q.Subject).WithMany().HasForeignKey(q => q.SubjectId).OnDelete(DeleteBehavior.Restrict);
            question.HasOne<ApplicationUser>().WithMany().HasForeignKey(q => q.AuthorId).OnDelete(DeleteBehavior.SetNull);
            question.HasMany(q => q.Options).WithOne().HasForeignKey(o => o.QuestionId).OnDelete(DeleteBehavior.Cascade);
            question.HasData(QuestionSeed.Questions.Select(q => new Question
            {
                Id = q.Id, Kind = q.Kind, SubjectId = q.SubjectId, Statement = q.Statement, Source = q.Source, CreatedAt = q.CreatedAt,
                ExpectedAnswer = q.ExpectedAnswer, SuggestedLines = q.SuggestedLines,
            }));
        });

        builder.Entity<QuestionOption>(option =>
        {
            option.Property(o => o.Text).HasMaxLength(2000);
            option.Property(o => o.Explanation).HasMaxLength(2000);
            option.HasIndex(o => new { o.QuestionId, o.Letter }).IsUnique();
            option.HasData(QuestionSeed.Options);
        });

        builder.Entity<QuestionAttempt>(attempt =>
        {
            attempt.HasIndex(a => new { a.UserId, a.QuestionId });
            attempt.HasOne<ApplicationUser>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            attempt.HasOne<Question>().WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Cascade);
            attempt.HasOne<QuestionOption>().WithMany().HasForeignKey(a => a.OptionId).OnDelete(DeleteBehavior.Cascade);
            attempt.HasOne<Challenge>().WithMany().HasForeignKey(a => a.ChallengeId).OnDelete(DeleteBehavior.Cascade);
            attempt.HasIndex(a => a.ChallengeId);
        });

        builder.Entity<Challenge>(challenge =>
        {
            challenge.Property(c => c.Title).HasMaxLength(ChallengeService.TitleMaxLength);
            challenge.Property(c => c.Description).HasMaxLength(ChallengeService.DescriptionMaxLength);
            challenge.HasOne<ApplicationUser>().WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.SetNull);
            challenge.HasMany(c => c.Questions).WithOne().HasForeignKey(q => q.ChallengeId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ChallengeQuestion>(question =>
        {
            question.HasKey(q => new { q.ChallengeId, q.QuestionId });
            question.HasOne(q => q.Question).WithMany().HasForeignKey(q => q.QuestionId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ChallengeSubmission>(submission =>
        {
            // Um envio por aluno em cada desafio.
            submission.HasKey(s => new { s.ChallengeId, s.UserId });
            submission.HasOne<Challenge>().WithMany().HasForeignKey(s => s.ChallengeId).OnDelete(DeleteBehavior.Cascade);
            submission.HasOne<ApplicationUser>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Post>(post =>
        {
            post.Property(p => p.Slug).HasMaxLength(Slugs.MaxLength + 10);
            post.Property(p => p.Title).HasMaxLength(BlogService.TitleMaxLength);
            post.Property(p => p.Summary).HasMaxLength(BlogService.SummaryMaxLength);
            post.Property(p => p.Body).HasMaxLength(BlogService.BodyMaxLength);
            post.Property(p => p.SourceUrl).HasMaxLength(BlogService.SourceMaxLength);
            post.HasIndex(p => p.Slug).IsUnique();
            post.HasIndex(p => p.PublishedAt);
            post.Ignore(p => p.IsPublished);
            post.HasOne<ApplicationUser>().WithMany().HasForeignKey(p => p.AuthorId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PostComment>(comment =>
        {
            comment.Property(c => c.Text).HasMaxLength(BlogService.CommentMaxLength);
            comment.HasIndex(c => c.PostId);
            comment.HasOne<Post>().WithMany().HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.Cascade);
            comment.HasOne<ApplicationUser>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // O SQLite (usado nos testes) não ordena nem compara DateTimeOffset; guarda como número só nele.
        // Receita da documentação do EF Core; no PostgreSQL nada muda.
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?)))
            {
                property.SetValueConverter(new DateTimeOffsetToBinaryConverter());
            }
        }

        builder.Entity<DiscursiveAnswer>(answer =>
        {
            answer.Property(a => a.Text).HasMaxLength(QuestionBank.DiscursiveAnswerMaxLength);
            answer.HasIndex(a => new { a.UserId, a.QuestionId });
            answer.HasOne<ApplicationUser>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            answer.HasOne<Question>().WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
