using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Data;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Mapping;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<TasksController> _logger;
    private readonly AppOptions _appOptions;

    public TasksController(AppDbContext db, ILogger<TasksController> logger, IOptions<AppOptions> appOptions)
    {
        _db = db;
        _logger = logger;
        _appOptions = appOptions.Value;
    }

    /// <summary>Get a page of tasks (newest first) with optional filters.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll(
        int page = 1,
        int pageSize = 0,
        bool? isDone = null,
        string? search = null,
        CancellationToken ct = default)
    {
        if (pageSize <= 0) pageSize = _appOptions.DefaultPageSize;
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > _appOptions.MaxPageSize) pageSize = _appOptions.MaxPageSize;

        _logger.LogInformation("GET /tasks page={Page} pageSize={PageSize} isDone={IsDone} search={Search}",
            page, pageSize, isDone, search);

        var query = _db.Tasks.AsQueryable();

        if (isDone.HasValue)
            query = query.Where(t => t.IsDone == isDone.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        query = query.OrderByDescending(t => t.CreatedUtc);

        var skip = (page - 1) * pageSize;

        // Project to DTOs in-memory (safe). For big queries, use .Select(...)
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(ct);
        var dtos = items.Select(e => e.ToReadDto()).ToList();

        _logger.LogDebug("Returning {Count} items", dtos.Count);
        return Ok(dtos);
    }

    /// <summary>Return a single task by ID, or 404 if not found.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskReadDto>> Get(int id, CancellationToken ct = default)
    {
        var item = await _db.Tasks.FindAsync([id], ct);
        if (item is null)
            return NotFound(new { message = $"Task {id} not found" });

        return Ok(item.ToReadDto());
    }

    /// <summary>Create a new task from JSON body.</summary>
    [HttpPost]
    public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto body, CancellationToken ct = default)
    {
        // [ApiController] + DataAnnotations on DTO → auto 400 on invalid model
        var entity = body.ToEntity();

        _db.Tasks.Add(entity);
        await _db.SaveChangesAsync(ct);

        var readDto = entity.ToReadDto();
        return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
    }

    /// <summary>Update an existing task by ID.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto body, CancellationToken ct = default)
    {
        if (id != body.Id) return BadRequest(new { message = "Route id must match body id." });

        var entity = await _db.Tasks.FindAsync([id], ct);
        if (entity is null) return NotFound(new { message = $"Task {id} not found" });

        entity.ApplyUpdate(body);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Delete a task by ID.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var entity = await _db.Tasks.FindAsync([id], ct);
        if (entity is null) return NotFound(new { message = $"Task {id} not found" });

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
