// -------------------------------------------------------
// TasksController.cs
// A simple REST controller using EF Core directly.
// (We'll introduce DTOs, services, and layers later.)
// -------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Base URL: /api/tasks
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    /// <summary>
    /// The DbContext is injected by ASP.NET Core's DI container.
    /// This gives us a configured AppDbContext per request.
    /// </summary>
    public TasksController(AppDbContext db) => _db = db;

    /// <summary>Return all tasks (newest first).</summary>
    /// <remarks>
    /// GET /api/tasks
    /// Uses LINQ to order by CreatedUtc DESC and materialize the list.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        // Build the query (IQueryable) and execute with ToListAsync()
        var items = await _db.Tasks
            .OrderByDescending(t => t.CreatedUtc)
            .ToListAsync();

        // ActionResult<T>: returning Ok(items) yields HTTP 200 + JSON body
        return Ok(items);
    }

    /// <summary>Return a single task by ID, or 404 if not found.</summary>
    /// <remarks>GET /api/tasks/5</remarks>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        // FindAsync uses the primary key and checks the change tracker first.
        var item = await _db.Tasks.FindAsync(id);
        if(item is null) return NotFound(new { message = $"Task {id} not found" }); // 404

        return Ok(item); // 200 with JSON body
    }

    /// <summary>Create a new task from JSON body.</summary>
    /// <remarks>
    /// POST /api/tasks
    /// Body example:
    /// { "title": "Finish Day 2", "description": "EF + CRUD", "isDone": false }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem body)
    {
        // Note: In this early stage we accept the entity directly.
        // Later we'll validate and use DTOs.
        _db.Tasks.Add(body);           // Stage INSERT
        await _db.SaveChangesAsync();  // Execute INSERT

        // Return 201 Created with a Location header to GET the new resource.
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    /// <summary>Update an existing task by ID.</summary>
    /// <remarks>
    /// PUT /api/tasks/5
    /// Body must include matching "id": 5
    /// </remarks>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItem body)
    {
        if (id != body.Id) return BadRequest(); // 400 if route id != body id

        // Attach the incoming entity and mark as Modified so EF generates UPDATE
        _db.Entry(body).State = EntityState.Modified;

        await _db.SaveChangesAsync(); // Execute UPDATE
        return NoContent();           // 204 (success, no body)
    }

    /// <summary>Delete a task by ID.</summary>
    /// <remarks>DELETE /api/tasks/5</remarks>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound(); // 404

        _db.Tasks.Remove(item);              // Stage DELETE
        await _db.SaveChangesAsync();        // Execute DELETE
        return NoContent();                  // 204
    }
}
