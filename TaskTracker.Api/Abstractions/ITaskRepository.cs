// -------------------------------------------------------
// ITaskRepository.cs
// Data access abstraction for tasks.
// -------------------------------------------------------
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Abstractions;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<TaskItem>> SearchAsync(
        int page, int pageSize, bool? isDone, string? search, CancellationToken ct = default);
    Task<int> CountAsync(bool? isDone, string? search, CancellationToken ct = default);
    Task AddAsync(TaskItem item, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    void Remove(TaskItem item);
}
