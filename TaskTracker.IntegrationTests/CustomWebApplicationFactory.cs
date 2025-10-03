using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;
using System.Linq;

namespace TaskTracker.IntegrationTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var overrides = new Dictionary<string, string>
            {
                ["Jwt:Issuer"] = "TaskTracker.IntegrationTests",
                ["Jwt:Audience"] = "TaskTracker.IntegrationTests",
                ["Jwt:Key"] = "SuperSecretIntegrationTestKey123!"
            };

            configBuilder.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory DB (still same name is fine)
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Ensure the service provider is created AFTER app starts
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                // Add project and task
                var project = new Project { Name = "Test Project" };
                db.Projects.Add(project);
                db.SaveChanges();

                db.Tasks.Add(new TaskItem
                {
                    Title = "Seed task",
                    ProjectId = project.Id
                });
                db.SaveChanges();
            }
        });

        builder.ConfigureTestServices(services =>
        {
            // Swap JWT for a deterministic test scheme that always authenticates
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.AddAuthorization(options =>
            {
                var testPolicy = new AuthorizationPolicyBuilder(TestAuthHandler.SchemeName)
                    .RequireAuthenticatedUser()
                    .Build();

                options.DefaultPolicy = testPolicy;
                options.FallbackPolicy = testPolicy;
            });
        });
    }
}
