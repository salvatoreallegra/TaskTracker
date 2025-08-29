using AutoMapper;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;

namespace TaskTracker.Api.Services;
public sealed class TaskService
{
    private readonly ITaskRepository _repo;
    private readonly AppOptions _options;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository repo, IOptions<AppOptions> options, IMapper mapper)
    {
        _repo = repo;
        _options = options.Value;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskReadDto>> SearchAsync(
        int page, int? pageSize, bool? isDone, string? search, CancellationToken ct = default)
    {
        var size = pageSize.GetValueOrDefault(_options.DefaultPageSize);
        if (page < 1) page = 1;
        if (size < 1) size = 1;
        if (size > _options.MaxPageSize) size = _options.MaxPageSize;

        var items = await _repo.SearchAsync(page, size, isDone, search, ct);
        return _mapper.Map<List<TaskReadDto>>(items);
    }

    public async Task<TaskReadDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return _mapper.Map<TaskReadDto?>(e);
    }

    public async Task<TaskReadDto> CreateAsync(TaskCreateDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<TaskItem>(dto);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<TaskReadDto>(entity);
    }

    public async Task<bool> UpdateAsync(int id, TaskUpdateDto dto, CancellationToken ct = default)
    {
        if (id != dto.Id) return false;

        var e = await _repo.GetByIdAsync(id, ct);
        if (e is null) return false;

        _mapper.Map(dto, e); // updates allowed fields
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
