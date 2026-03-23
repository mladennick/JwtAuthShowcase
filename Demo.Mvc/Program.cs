using Demo.Mvc.Data;
using JwtAuth.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events ??= new JwtBearerEvents();
    options.Events.OnTokenValidated = async context =>
    {
        var jwtId = context.Principal?.FindFirst("jti")?.Value;
        if (string.IsNullOrWhiteSpace(jwtId))
        {
            context.Fail("Token is missing jti claim.");
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var session = await dbContext.UserSessions.AsNoTracking().SingleOrDefaultAsync(s => s.JwtId == jwtId);
        if (session is null)
        {
            context.Fail("Session not found.");
            return;
        }

        if (session.IsRevoked)
        {
            context.Fail("Session is revoked.");
            return;
        }

        if (session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            context.Fail("Session has expired.");
        }
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
