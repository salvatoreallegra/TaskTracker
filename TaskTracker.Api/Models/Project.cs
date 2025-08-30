// -------------------------------------------------------
// Project.cs
// PURPOSE: Represents a collection of tasks under a project.
// Demonstrates a 1-to-many EF Core relationship.
// -------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

public sealed class Project
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = "";

    // Navigation property: one project has many tasks
    public List<TaskItem> Tasks { get; set; } = new();
}
