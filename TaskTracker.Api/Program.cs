using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Data;
using TaskTracker.Api.Middleware;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;
using TaskTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== MVC / Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Keep your doc visible and add Bearer support so Swagger shows the Authorize button
    c.SwaggerDoc("v1", new() { Title = "TaskTracker API", Version = "v1" });

    // Bearer JWT security scheme
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===== DI =====
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ===== CORS: dev client origins =====
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

// ===== Options =====
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));

// ===== DbContext registration with safe fallback =====
// Tests: keep Testing environment free to override in CustomWebApplicationFactory.
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

// ===== JWT Auth setup (must be BEFORE Build) =====
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtKey = jwtSection["Key"]; // dev secret; move to App Service / Key Vault in prod

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ===== Seeding & Migrations (minimal, safe) =====
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Database.IsInMemory())
    {
        // Apply EF Core migrations automatically in dev/compose/cloud
        db.Database.Migrate();

        // Dev-only seed if empty
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
        // InMemory fallback: tiny demo seed
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

// IMPORTANT: Authentication must come BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// For WebApplicationFactory<T> in integration tests
public partial class Program { }
