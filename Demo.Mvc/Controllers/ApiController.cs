using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Mvc.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var username = User.Identity?.Name ?? User.FindFirst("unique_name")?.Value ?? "unknown";
        var jwtId = User.FindFirst("jti")?.Value ?? string.Empty;

        return Ok(new
        {
            username,
            jwtId,
            message = "Protected endpoint reached with a valid and active session."
        });
    }
}
