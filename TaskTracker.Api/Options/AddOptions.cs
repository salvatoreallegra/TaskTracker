// -------------------------------------------------------
// AppOptions.cs
// Typed configuration for application-wide settings.
// Bound from "App" section in appsettings.json.
// -------------------------------------------------------
namespace TaskTracker.Api.Options;

public sealed class AppOptions
{
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}
