// -------------------------------------------------------
// TaskRepository.cs
// EF Core implementation of ITaskRepository.
// -------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data;

public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;
    public TaskRepository(AppDbContext db) => _db = db;

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Tasks.FindAsync([id], ct);

    public async Task<List<TaskItem>> SearchAsync(
        int page, int pageSize, bool? isDone, string? search, CancellationToken ct = default)
    {
        var q = _db.Tasks.AsQueryable();

        if (isDone.HasValue) q = q.Where(t => t.IsDone == isDone.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            q = q.Where(t => t.Title.ToLower().Contains(term) ||
                             (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        q = q.OrderByDescending(t => t.CreatedUtc);

        var skip = (page - 1) * pageSize;
        return await q.Skip(skip).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(bool? isDone, string? search, CancellationToken ct = default)
    {
        var q = _db.Tasks.AsQueryable();

        if (isDone.HasValue) q = q.Where(t => t.IsDone == isDone.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            q = q.Where(t => t.Title.ToLower().Contains(term) ||
                             (t.Description != null && t.Description.ToLower().Contains(term)));
        }
        return await q.CountAsync(ct);
    }

    public async Task AddAsync(TaskItem item, CancellationToken ct = default)
    {
        _db.Tasks.Add(item);
        await Task.CompletedTask;
    }

    public void Remove(TaskItem item) => _db.Tasks.Remove(item);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
