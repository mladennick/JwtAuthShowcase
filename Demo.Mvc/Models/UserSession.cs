namespace Demo.Mvc.Models;

/// <summary>
/// Tracks issued JWT sessions for users in the demo application.
/// </summary>
public class UserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    public string JwtId { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAtUtc { get; set; }

    public bool IsRevoked { get; set; }
}
