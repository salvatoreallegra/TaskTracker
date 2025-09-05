using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; // Default role

    // Navigation property
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
