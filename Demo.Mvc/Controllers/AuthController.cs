using Demo.Mvc.Data;
using Demo.Mvc.Models;
using Demo.Mvc.Models.Auth;
using Demo.Mvc.Utilities;
using JwtAuth.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Demo.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(AppDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var usernameExists = await _dbContext.Users.AnyAsync(u => u.Username == username);
        if (usernameExists)
        {
            ModelState.AddModelError(nameof(model.Username), "Username is already in use.");
            return View(model);
        }

        var user = new AppUser
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(model.Password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Registration completed. You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var passwordHash = PasswordHasher.Hash(model.Password);
        var user = await _dbContext.Users.SingleOrDefaultAsync(u =>
            u.Username == username && u.PasswordHash == passwordHash);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Username);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var jwtId = jwt.Id;
        var expiresAtUtc = jwt.ValidTo;

        _dbContext.UserSessions.Add(new UserSession
        {
            AppUserId = user.Id,
            JwtId = jwtId,
            ExpiresAtUtc = expiresAtUtc
        });
        await _dbContext.SaveChangesAsync();

        var result = new LoginResultViewModel
        {
            Username = user.Username,
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            JwtId = jwtId
        };

        return View("LoginResult", result);
    }

    [HttpGet]
    public async Task<IActionResult> Sessions()
    {
        var sessions = await _dbContext.UserSessions
            .AsNoTracking()
            .Include(s => s.AppUser)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Take(50)
            .ToListAsync();

        return View(sessions);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var session = await _dbContext.UserSessions.SingleOrDefaultAsync(s => s.Id == id);
        if (session is null)
        {
            TempData["ErrorMessage"] = "Session not found.";
            return RedirectToAction(nameof(Sessions));
        }

        if (!session.IsRevoked)
        {
            session.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Session revoked successfully.";
        }
        else
        {
            TempData["SuccessMessage"] = "Session is already revoked.";
        }

        return RedirectToAction(nameof(Sessions));
    }
}
