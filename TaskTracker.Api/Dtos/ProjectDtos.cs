namespace TaskTracker.Api.Dtos;

public sealed class ProjectReadDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public List<TaskReadDto> Tasks { get; init; } = new();
}

public sealed class ProjectCreateDto
{
    public string Name { get; init; } = "";
}
