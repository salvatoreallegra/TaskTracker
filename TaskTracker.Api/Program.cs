using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Data;
using TaskTracker.Api.Middleware;
using TaskTracker.Api.Options;
using TaskTracker.Api.Services;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// CORS: define a named policy for your dev client origin
const string DevClientCors = "DevClientCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DevClientCors, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // your React dev hosts
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

if (builder.Environment.IsEnvironment("Testing"))
{
    // Register dummy provider in tests (we'll override it in CustomWebApplicationFactory anyway)
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
}


builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));
var app = builder.Build();

// ===== DEV-ONLY DB SEEDING =====
/*if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db); // seed dev data
}*/
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must be before MapControllers
app.UseCors(DevClientCors);

app.UseGlobalExceptionHandling();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
public partial class Program { } // Needed for WebApplicationFactory<T>

