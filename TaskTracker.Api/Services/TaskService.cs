// -------------------------------------------------------
// TaskService.cs
// Business/use-case logic for tasks.
// -------------------------------------------------------
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Mapping;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;
using Microsoft.Extensions.Options;

namespace TaskTracker.Api.Services;

public sealed class TaskService
{
    private readonly ITaskRepository _repo;
    private readonly AppOptions _options;

    public TaskService(ITaskRepository repo, IOptions<AppOptions> options)
    {
        _repo = repo;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<TaskReadDto>> SearchAsync(
        int page, int? pageSize, bool? isDone, string? search, CancellationToken ct = default)
    {
        var size = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : _options.DefaultPageSize;
        if (page < 1) page = 1;
        if (size < 1) size = 1;
        if (size > _options.MaxPageSize) size = _options.MaxPageSize;

        var items = await _repo.SearchAsync(page, size, isDone, search, ct);
        return items.Select(e => e.ToReadDto()).ToList();
    }

    public async Task<TaskReadDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e?.ToReadDto();
    }

    public async Task<TaskReadDto> CreateAsync(TaskCreateDto dto, CancellationToken ct = default)
    {
        var entity = dto.ToEntity();
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.ToReadDto();
    }

    public async Task<bool> UpdateAsync(int id, TaskUpdateDto dto, CancellationToken ct = default)
    {
        if (id != dto.Id) return false;

        var e = await _repo.GetByIdAsync(id, ct);
        if (e is null) return false;

        e.ApplyUpdate(dto);
        await _repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e is null) return false;

        _repo.Remove(e);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}
