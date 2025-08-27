// -------------------------------------------------------
// AppDbContext.cs
// Manages EF Core database access for TaskTracker.
// Defines tables (DbSets) and configures schema.
// -------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data;

/// <summary>
/// EF Core database context. Represents a session with the database.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Tasks table represented as a DbSet.</summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure schema rules
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(2000);
        });
    }
}
