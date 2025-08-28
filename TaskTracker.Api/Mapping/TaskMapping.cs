// -------------------------------------------------------
// TaskMapping.cs
// Manual mapping between Entity <-> DTOs.
// Keep these small and obvious for now.
// -------------------------------------------------------
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Mapping;

public static class TaskMapping
{
    public static TaskReadDto ToReadDto(this TaskItem e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        IsDone = e.IsDone,
        CreatedUtc = e.CreatedUtc,
        DueUtc = e.DueUtc
    };

    public static TaskItem ToEntity(this TaskCreateDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        IsDone = false,                  // server decides
        CreatedUtc = DateTime.UtcNow,    // server decides
        DueUtc = dto.DueUtc
    };

    public static void ApplyUpdate(this TaskItem e, TaskUpdateDto dto)
    {
        // apply allowed changes
        e.Title = dto.Title;
        e.Description = dto.Description;
        e.IsDone = dto.IsDone;
        e.DueUtc = dto.DueUtc;
        // CreatedUtc remains unchanged
    }
}
