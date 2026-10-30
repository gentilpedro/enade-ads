using EnadeAds.Questions;
using EnadeAds.Study;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RoadmapNode> RoadmapNodes => Set<RoadmapNode>();
    public DbSet<RoadmapProgress> RoadmapProgress => Set<RoadmapProgress>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<QuestionAttempt> QuestionAttempts => Set<QuestionAttempt>();
    public DbSet<DiscursiveAnswer> DiscursiveAnswers => Set<DiscursiveAnswer>();

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
        });

        builder.Entity<DiscursiveAnswer>(answer =>
        {
            answer.Property(a => a.Text).HasMaxLength(QuestionBank.DiscursiveAnswerMaxLength);
            answer.HasIndex(a => new { a.UserId, a.QuestionId });
            answer.HasOne<ApplicationUser>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            answer.HasOne<Question>().WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
