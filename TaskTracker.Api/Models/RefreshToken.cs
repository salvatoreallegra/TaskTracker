using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresUtc { get; set; }
    public bool IsRevoked { get; set; }

    // Link to User
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
