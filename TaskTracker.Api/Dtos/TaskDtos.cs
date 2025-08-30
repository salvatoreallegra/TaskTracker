// -------------------------------------------------------
// TaskDtos.cs
// API contracts for TaskItem endpoints.
// - Read DTO: what we RETURN to clients.
// - Create/Update DTO: what clients SEND to us.
// -------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

/// <summary>Shape returned to clients.</summary>
public sealed class TaskReadDto
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public string? Description { get; init; }
    public bool IsDone { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? DueUtc { get; init; }
}

/// <summary>Shape clients POST to create a task.</summary>
public sealed class TaskCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = "";

    [MaxLength(2000)]
    public string? Description { get; init; }

    public DateTime? DueUtc { get; init; }

    [Required]
    public int ProjectId { get; init; }
}

/// <summary>Shape clients PUT to update an existing task.</summary>
public sealed class TaskUpdateDto
{
    [Required]
    public int Id { get; init; }

    [Required, MaxLength(200)]
    public string Title { get; init; } = "";

    [MaxLength(2000)]
    public string? Description { get; init; }

    public bool IsDone { get; init; }
    public DateTime? DueUtc { get; init; }
}
