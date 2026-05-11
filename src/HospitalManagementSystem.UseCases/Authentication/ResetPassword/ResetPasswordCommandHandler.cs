using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, bool>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async ValueTask<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
  {
    try
    {
      if (!Guid.TryParse(request.UserId, out var userId))
        return false;

      var user = await _userManager.FindByIdAsync(userId.ToString());

      if (user == null)
        return false;

      // Validate token
      if (user.PasswordResetToken != request.Token)
        return false;

      if (!user.PasswordResetTokenExpiryAt.HasValue || user.PasswordResetTokenExpiryAt < DateTime.UtcNow)
        return false;

      // Remove password
      await _userManager.RemovePasswordAsync(user);

      // Add new password
      var result = await _userManager.AddPasswordAsync(user, request.NewPassword);

      if (!result.Succeeded)
        return false;

      // Clear reset token
      user.PasswordResetToken = null!;
      user.PasswordResetTokenExpiryAt = null;
      user.PasswordChangeCount++;
      user.LastPasswordChangedAt = DateTime.UtcNow;

      await _userManager.UpdateAsync(user);

      return true;
    }
    catch
    {
      return false;
    }
  }
}
