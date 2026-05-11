using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Authentication.Dtos;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.GetAuthStatus;

public class GetAuthStatusQueryHandler : IQueryHandler<GetAuthStatusQuery, AuthStatusResponse>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public GetAuthStatusQueryHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async ValueTask<AuthStatusResponse> Handle(GetAuthStatusQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var user = await _userManager.FindByIdAsync(request.UserId);

      if (user == null)
      {
        return new AuthStatusResponse
        {
          IsAuthenticated = false
        };
      }

      return new AuthStatusResponse
      {
        IsAuthenticated = true,
        UserId = user.Id.ToString(),
        Email = user.Email!,
        EmailVerified = user.EmailVerifiedAt.HasValue,
        IsAccountLocked = user.IsAccountLocked,
        AccessFailedCount = user.AccessFailedCount,
        TokenExpires = DateTime.UtcNow.AddMinutes(15)
      };
    }
    catch
    {
      return new AuthStatusResponse { IsAuthenticated = false };
    }
  }
}
