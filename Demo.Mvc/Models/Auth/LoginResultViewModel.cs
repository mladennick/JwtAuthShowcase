namespace Demo.Mvc.Models.Auth;

public class LoginResultViewModel
{
    public string Username { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public string JwtId { get; set; } = string.Empty;
}
