using HospitalManagementSystem.Core.HospitalAggregate;
using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Infrastructure.Data;

public static class IdentitySeeder
{
  private static readonly Guid DowntownHospitalId = Guid.Parse("11111111-1111-1111-1111-111111111111");
  private static readonly Guid WestHospitalId = Guid.Parse("22222222-2222-2222-2222-222222222222");

  public static async Task SeedAsync(
      AppDbContext dbContext,
      UserManager<ApplicationUser> userManager,
      RoleManager<ApplicationRole> roleManager)
  {
    var downtownHospital = await EnsureHospitalAsync(dbContext, DowntownHospitalId, "Meridian Downtown Hospital", "MERIDIAN-DOWNTOWN");
    var westHospital = await EnsureHospitalAsync(dbContext, WestHospitalId, "Meridian West Clinic", "MERIDIAN-WEST");

    string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };

    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new ApplicationRole { Role = role, Name = role });
      }
    }

    var adminEmail = "admin@hospital.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
      var user = new ApplicationUser
      {
        UserName = adminEmail,
        Email = adminEmail,
        FullName = "Super Admin",
        HospitalId = null
      };

      await userManager.CreateAsync(user, "Admin@123");
      await userManager.AddToRoleAsync(user, "Admin");
    }
    else
    {
      if (adminUser.HospitalId is not null)
      {
        adminUser.HospitalId = null;
        await userManager.UpdateAsync(adminUser);
      }

      if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
      {
        await userManager.AddToRoleAsync(adminUser, "Admin");
      }
    }

    await EnsureUserAsync(userManager, "doctor@hospital.com", "doctor@hospital.com", "Default Doctor", "Doctor@123", "Doctor", downtownHospital.Id);
    await EnsureUserAsync(userManager, "receptionist@hospital.com", "receptionist@hospital.com", "Default Receptionist", "Receptionist@123", "Receptionist", downtownHospital.Id);
    await EnsureUserAsync(userManager, "patient@hospital.com", "patient@hospital.com", "Default Patient", "Patient@123", "Patient", westHospital.Id);

    await EnsureUserAsync(userManager, "doctor-west@hospital.com", "doctor-west@hospital.com", "Default West Doctor", "Doctor@123", "Doctor", westHospital.Id);
    await EnsureUserAsync(userManager, "receptionist-west@hospital.com", "receptionist-west@hospital.com", "Default West Receptionist", "Receptionist@123", "Receptionist", westHospital.Id);
    await EnsureUserAsync(userManager, "patient-west@hospital.com", "patient-west@hospital.com", "Default West Patient", "Patient@123", "Patient", westHospital.Id);
  }

  private static async Task<Hospital> EnsureHospitalAsync(AppDbContext dbContext, Guid id, string name, string code)
  {
    var existing = await dbContext.Hospitals.FirstOrDefaultAsync(x => x.Code == code);
    if (existing is not null)
    {
      return existing;
    }

    var hospital = new HospitalManagementSystem.Core.HospitalAggregate.Hospital(id, name, code);
    dbContext.Hospitals.Add(hospital);
    await dbContext.SaveChangesAsync();
    return hospital;
  }

  private static async Task EnsureUserAsync(
    UserManager<ApplicationUser> userManager,
    string email,
    string userName,
    string fullName,
    string password,
    string role,
    Guid hospitalId)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is not null)
    {
      var shouldUpdate = false;
      if (user.HospitalId != hospitalId)
      {
        user.HospitalId = hospitalId;
        shouldUpdate = true;
      }

      if (!await userManager.IsInRoleAsync(user, role))
      {
        await userManager.AddToRoleAsync(user, role);
      }

      if (shouldUpdate)
      {
        await userManager.UpdateAsync(user);
      }

      return;
    }

    user = new ApplicationUser
    {
      UserName = userName,
      Email = email,
      FullName = fullName,
      HospitalId = hospitalId
    };

    await userManager.CreateAsync(user, password);
    await userManager.AddToRoleAsync(user, role);
  }
}
