using Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain;

public class AppDbContext : IdentityDbContext<IdentityUser>
{

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    var userGuid = Guid.NewGuid().ToString();
    var roleGuid = Guid.NewGuid().ToString();

    modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
    {
      Id=roleGuid,
      Name="Admin",
      NormalizedName="ADMIN"
    });

    modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser
    {
      Id=userGuid,
      UserName="Admin",
      NormalizedUserName="ADMIN",
      Email="admin@email.com",
      NormalizedEmail="ADMIN@EMAIL.COM",
      EmailConfirmed=true,
      PasswordHash=new PasswordHasher<IdentityUser>().HashPassword(new IdentityUser(), "hackOFF1"),
      SecurityStamp=string.Empty
    });

    modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
    {
      RoleId=roleGuid,
      UserId=userGuid,
    });
  }

  public DbSet<Post> Posts { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<IdentityUserRole<string>> UserRole { get; set; }
}

