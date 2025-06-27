using DevLife.Backend.Domain;
using DevLife.Backend.Modules.BugChase;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Backend.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<CodeSnippet> CodeSnippets => Set<CodeSnippet>();
    public DbSet<Score> Scores => Set<Score>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Score>()
            .HasOne(s => s.User)
            .WithOne(u => u.Score)
            .HasForeignKey<Score>(s => s.UserId) 
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Score>()
            .HasIndex(s => s.UserId)
            .IsUnique();
    }
}
