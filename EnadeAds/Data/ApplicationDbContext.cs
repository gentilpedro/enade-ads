using EnadeAds.Study;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RoadmapNode> RoadmapNodes => Set<RoadmapNode>();
    public DbSet<RoadmapProgress> RoadmapProgress => Set<RoadmapProgress>();

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
    }
}
