namespace TaskTracker.Api.Models
{
    /// <summary>
    /// Represents a single task in the Task Tracker system.
    /// </summary>
    public class TaskItem
    {
        /// <summary>Primary key ID (auto-increment).</summary>
        public int Id { get; set; }

        /// <summary>Title of the task (required, max 200 chars).</summary>
        public string Title { get; set; } = "";

        /// <summary>Optional task description (max 2000 chars).</summary>
        public string? Description { get; set; }

        /// <summary>Whether the task is complete.</summary>
        public bool IsDone { get; set; }

        /// <summary>When the task was created (UTC).</summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Optional due date (UTC).</summary>
        public DateTime? DueUtc { get; set; }
    }
}
