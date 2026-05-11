using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.VerifyEmail;

public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, bool>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailVerificationService _emailVerificationService;

  public VerifyEmailCommandHandler(
      UserManager<ApplicationUser> userManager,
      IEmailVerificationService emailVerificationService)
  {
    _userManager = userManager;
    _emailVerificationService = emailVerificationService;
  }

  public async ValueTask<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
  {
    try
    {
      if (!Guid.TryParse(request.UserId, out var userId))
        return false;

      var user = await _userManager.FindByIdAsync(userId.ToString());

      if (user == null)
        return false;

      return await _emailVerificationService.VerifyEmailAsync(user, request.Token);
    }
    catch
    {
      return false;
    }
  }
}
