// -------------------------------------------------------
// ProjectsController.cs
// PURPOSE: Manage projects (with tasks).
// Demonstrates returning nested DTOs (Project -> Tasks).
// -------------------------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskTracker.Api.Data;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ProjectsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetAll(CancellationToken ct)
    {
        var projects = await _db.Projects
            .Include(p => p.Tasks) // eager load tasks
            .ToListAsync(ct);
        return Ok(_mapper.Map<List<ProjectReadDto>>(projects));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectReadDto>> Get(int id, CancellationToken ct)
    {
        var project = await _db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (project is null) return NotFound();
        return Ok(_mapper.Map<ProjectReadDto>(project));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> Create(ProjectCreateDto dto, CancellationToken ct)
    {
        var entity = _mapper.Map<Project>(dto);
        _db.Projects.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<ProjectReadDto>(entity));
    }
}
