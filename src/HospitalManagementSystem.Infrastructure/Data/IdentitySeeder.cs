using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;
namespace HospitalManagementSystem.Infrastructure.Data;

public static class IdentitySeeder
{
  public static async Task SeedAsync(
      UserManager<ApplicationUser> userManager,
      RoleManager<ApplicationRole> roleManager)
  {
    // 1. Seed Roles
    string[] roles = { "Admin", "Doctor", "Patient" };

    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new ApplicationRole { Role = role, Name=role });
      }
    }

    // 2. Seed Super Admin
    var adminEmail = "admin@hospital.com";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
      var user = new ApplicationUser
      {
        UserName = adminEmail,
        Email = adminEmail,
        FullName = "Super Admin"
      };

      await userManager.CreateAsync(user, "Admin@123");

      await userManager.AddToRoleAsync(user, "Admin");
    }

    // 3. Seed Doctor
    var doctorEmail = "doctor@hospital.com";

    var doctorUser = await userManager.FindByEmailAsync(doctorEmail);

    if (doctorUser == null)
    {
      var user = new ApplicationUser
      {
        UserName = doctorEmail,
        Email = doctorEmail,
        FullName = "Default Doctor"
      };

      await userManager.CreateAsync(user, "Doctor@123");

      await userManager.AddToRoleAsync(user, "Doctor");
    }

    // 4. Seed Patient
    var patientEmail = "patient@hospital.com";

    var patientUser = await userManager.FindByEmailAsync(patientEmail);

    if (patientUser == null)
    {
      var user = new ApplicationUser
      {
        UserName = patientEmail,
        Email = patientEmail,
        FullName = "Default Patient"
      };

      await userManager.CreateAsync(user, "Patient@123");

      await userManager.AddToRoleAsync(user, "Patient");
    }
  }
}
