using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// ===== DEV-ONLY DB SEEDING =====
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db); // seed dev data
}
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must be before MapControllers
app.UseCors(DevClientCors);

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
