namespace Demo.Mvc.Models;

/// <summary>
/// Represents an application user persisted by the demo app.
/// </summary>
public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}
