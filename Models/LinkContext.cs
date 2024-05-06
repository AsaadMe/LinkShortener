
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Models;

public class LinkContext : DbContext
{
    public DbSet<Link> Links { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;database=linkshortener;user=root;password=pass");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Link>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.HasIndex(e => e.LongUrl).IsUnique();
            entity.Property(e => e.ShortKey).IsRequired();
            entity.Property(e => e.CreationTime);
        });
    }
}
