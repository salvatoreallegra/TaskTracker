// -------------------------------------------------------
// TaskApiTests.cs
// PURPOSE: Ensure /api/tasks endpoints work end-to-end.
// -------------------------------------------------------
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using TaskTracker.Api.Dtos;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace TaskTracker.IntegrationTests;

public class TaskApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaskApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

   /* [Fact]
    public async Task GetAll_ReturnsSeedTask()
    {
        var response = await _client.GetAsync("/api/tasks?page=1&pageSize=10");

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskReadDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tasks);
        Assert.Contains(tasks, t => t.Title == "Seed task");
    }*/

   /* [Fact]
    public async Task Post_CreatesTask()
    {
        var createDto = new TaskCreateDto
        {
            Title = "Integration created",
            ProjectId = 1 // Assuming ProjectId=1 is seeded
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var created = await response.Content.ReadFromJsonAsync<TaskReadDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("Integration created", created!.Title);
    }*/
}
