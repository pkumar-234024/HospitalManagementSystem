using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.ContributorAggregate;
using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HospitalManagementSystem.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
  public DbSet<Appointment> Appointments => Set<Appointment>();
  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<UserEmail> UserEmails => Set<UserEmail>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    modelBuilder.Entity<ApplicationUser>().ToTable("Users");
    modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
    modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
    modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
    modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
    modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
    modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
