using Demo.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.JwtId).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.JwtId).IsUnique();
            entity.HasOne(x => x.AppUser)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
