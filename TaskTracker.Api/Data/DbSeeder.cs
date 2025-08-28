// -------------------------------------------------------
// DbSeeder.cs
// Seeds predictable dev data (idempotent).
// Call from Program.cs only in Development.
// -------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        // Ensure DB exists/migrated
        await db.Database.MigrateAsync(ct);

        // Seed only if empty
        if (!await db.Tasks.AnyAsync(ct))
        {
            db.Tasks.AddRange(
                new TaskItem { Title = "Learn EF Core", Description = "Models, DbContext, Migrations", IsDone = false },
                new TaskItem { Title = "Add Swagger", Description = "Explore endpoints", IsDone = true },
                new TaskItem { Title = "Implement CORS", Description = "Allow frontend dev origin", IsDone = false },
                new TaskItem { Title = "Seed Data", Description = "Make dev easier", IsDone = false }
            );
            await db.SaveChangesAsync(ct);
        }
    }
}
