using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Data;
using TaskTracker.Api.Middleware;
using TaskTracker.Api.Options;
using TaskTracker.Api.Services;
using TaskTracker.Api.Models; // for TaskItem

var builder = WebApplication.CreateBuilder(args);

// MVC / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// CORS: dev client origins
const string DevClientCors = "DevClientCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DevClientCors, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Options
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));

// ===== DbContext registration with safe fallback =====
// Tests: keep your Testing environment free to override in CustomWebApplicationFactory.
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connStr = builder.Configuration.GetConnectionString("Default");

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        if (string.IsNullOrWhiteSpace(connStr))
        {
            // No connection string? Use in-memory so the app still boots (useful for Azure/AppService misconfig or quick demos)
            options.UseInMemoryDatabase("TaskTrackerInMemory");
        }
        else
        {
            options.UseSqlServer(connStr);
        }
    });
}
else
{
    // In tests we intentionally don't register a provider here;
    // CustomWebApplicationFactory should replace/override the DbContext as needed.
}

// Build app
var app = builder.Build();

// ===== Seeding =====
// Keep your dev seeding, and also seed a tiny demo set if we're on InMemory (only if not Testing)
// ===== Seeding & Migrations =====
// ===== Seeding & Migrations =====
// ===== Seeding & Migrations (minimal, safe) =====
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Database.IsInMemory())
    {
        db.Database.Migrate();

        if (app.Environment.IsDevelopment() && !db.Projects.Any())
        {
            var p1 = new Project { Name = "Website Revamp" };
            var p2 = new Project { Name = "Mobile App" };
            db.Projects.AddRange(p1, p2);
            await db.SaveChangesAsync();

            db.Tasks.AddRange(
                new TaskItem { Title = "Seeded (SQL) — Task 1", ProjectId = p1.Id },
                new TaskItem { Title = "Seeded (SQL) — Task 2", ProjectId = p2.Id }
            );
            await db.SaveChangesAsync();
        }
    }
    else
    {
        if (!db.Projects.Any())
        {
            var p = new Project { Name = "InMemory Project" };
            db.Projects.Add(p);
            await db.SaveChangesAsync();

            if (!db.Tasks.Any())
            {
                db.Tasks.AddRange(
                    new TaskItem { Title = "Hello from InMemory", ProjectId = p.Id },
                    new TaskItem { Title = "Azure App Service demo", ProjectId = p.Id }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}




// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(DevClientCors);

app.UseGlobalExceptionHandling();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// For WebApplicationFactory<T> in integration tests
public partial class Program { }
