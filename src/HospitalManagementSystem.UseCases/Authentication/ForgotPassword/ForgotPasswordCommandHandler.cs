using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HospitalManagementSystem.UseCases.Authentication.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, bool>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IEmailService _emailService;
  private readonly EmailOptions _emailOptions;

  public ForgotPasswordCommandHandler(
      UserManager<ApplicationUser> userManager,
      IJwtTokenService jwtTokenService,
      IEmailService emailService,
      IOptions<EmailOptions> emailOptions)
  {
    _userManager = userManager;
    _jwtTokenService = jwtTokenService;
    _emailService = emailService;
    _emailOptions = emailOptions.Value;
  }

  public async ValueTask<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null || !user.EmailConfirmed)
        return false; // Don't reveal if user exists

      var resetToken = _jwtTokenService.GeneratePasswordResetToken(user);
      user.PasswordResetToken = resetToken;
      user.PasswordResetTokenExpiryAt = DateTime.UtcNow.AddMinutes(_emailOptions.PasswordResetTokenExpirationMinutes);

      await _userManager.UpdateAsync(user);

      var resetLink = $"https://yourdomain.com/reset-password?userId={user.Id}&token={Uri.EscapeDataString(resetToken)}";

      await _emailService.SendEmailAsync(
          user.Email!,
          "Password Reset Request",
          $@"
                        <h2>Password Reset</h2>
                        <p>Click the link below to reset your password:</p>
                        <a href='{resetLink}'>Reset Password</a>
                        <p>This link will expire in {_emailOptions.PasswordResetTokenExpirationMinutes} minutes.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                    "
      );

      return true;
    }
    catch
    {
      return false;
    }
  }
}
