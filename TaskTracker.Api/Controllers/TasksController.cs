using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Services;


namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _service;
    private readonly ILogger<TasksController> _logger;

    public TasksController(TaskService service, ILogger<TasksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll(
        int page = 1, int pageSize = 0, bool? isDone = null, string? search = null, CancellationToken ct = default)
    {
        _logger.LogInformation("GET /tasks page={Page} pageSize={PageSize} ...", page, pageSize);
        var result = await _service.SearchAsync(page, pageSize, isDone, search, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskReadDto>> Get(int id, CancellationToken ct = default)
    {
        var dto = await _service.GetAsync(id, ct);
        return dto is null ? NotFound(new { message = $"Task {id} not found" }) : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto body, CancellationToken ct = default)
    {
        var created = await _service.CreateAsync(body, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto body, CancellationToken ct = default)
    {
        var ok = await _service.UpdateAsync(id, body, ct);
        if (!ok) return BadRequest(new { message = "Invalid id or task not found." });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound(new { message = $"Task {id} not found" });
    }
}

