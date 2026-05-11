using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.AdminResetPassword;

public class AdminResetPasswordCommandHandler : ICommandHandler<AdminResetPasswordCommand, bool>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;

  public AdminResetPasswordCommandHandler(
      UserManager<ApplicationUser> userManager,
      IEmailService emailService)
  {
    _userManager = userManager;
    _emailService = emailService;
  }

  public async ValueTask<bool> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
  {
    try
    {
      if (!Guid.TryParse(request.UserId, out var userId))
        return false;

      var user = await _userManager.FindByIdAsync(userId.ToString());

      if (user == null)
        return false;

      // Verify admin permissions (should be checked in controller/middleware)
      var admin = await _userManager.FindByIdAsync(request.AdminId);
      if (admin == null)
        return false;

      var adminRoles = await _userManager.GetRolesAsync(admin);
      if (!adminRoles.Contains("Admin"))
        return false;

      // Remove old password
      await _userManager.RemovePasswordAsync(user);

      // Set temporary password
      var tempPassword = GenerateTemporaryPassword();
      var result = await _userManager.AddPasswordAsync(user, tempPassword);

      if (!result.Succeeded)
        return false;

      user.LastPasswordChangedAt = DateTime.UtcNow;
      user.PasswordChangeCount++;
      await _userManager.UpdateAsync(user);

      // Send temporary password email
      await _emailService.SendEmailAsync(
          user.Email!,
          "Password Reset by Administrator",
          $@"
                        <h2>Password Reset</h2>
                        <p>An administrator has reset your password.</p>
                        <p>Your temporary password is: <strong>{tempPassword}</strong></p>
                        <p>Please change this password immediately upon login.</p>
                    "
      );

      return true;
    }
    catch
    {
      return false;
    }
  }

  private string GenerateTemporaryPassword()
  {
    return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(12))
        .Substring(0, 12);
  }
}
