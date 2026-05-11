using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Dtos.Login;
using HospitalManagementSystem.UseCases.Dtos.User;
using Microsoft.AspNetCore.Identity;
namespace HospitalManagementSystem.UseCases.Authentication.GoogleAuth;
public class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, LoginResponse>
{
  private readonly IGmailAuthenticationService _gmailAuthService;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly UserManager<ApplicationUser> _userManager;

  public GoogleAuthCommandHandler(
      IGmailAuthenticationService gmailAuthService,
      IJwtTokenService jwtTokenService,
      IRefreshTokenService refreshTokenService,
      UserManager<ApplicationUser> userManager)
  {
    _gmailAuthService = gmailAuthService;
    _jwtTokenService = jwtTokenService;
    _refreshTokenService = refreshTokenService;
    _userManager = userManager;
  }

  public async ValueTask<LoginResponse> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var (success, user, message) = await _gmailAuthService.AuthenticateWithGoogleTokenAsync(request.IdToken);

      if (!success || user == null)
      {
        return new LoginResponse
        {
          Success = false,
          Message = message
        };
      }

      if (!user.IsActive)
      {
        return new LoginResponse
        {
          Success = false,
          Message = "User account is inactive"
        };
      }

      var roles = await _userManager.GetRolesAsync(user);
      var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
      var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
          user.Id, request.IpAddress, request.UserAgent);

      return new LoginResponse
      {
        Success = true,
        Message = "Google authentication successful",
        User = MapUserToDto(user),
        AccessToken = accessToken,
        RefreshToken = refreshToken.Token,
        AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpires = refreshToken.ExpiresAt,
        EmailVerified = user.EmailVerifiedAt.HasValue
      };
    }
    catch (Exception ex)
    {
      return new LoginResponse
      {
        Success = false,
        Message = $"Google authentication failed: {ex.Message}"
      };
    }
  }

  private UserDto MapUserToDto(ApplicationUser user)
  {
    return new UserDto
    {
      Id = user.Id.ToString(),
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email!,
      PhoneNumber = user.PhoneNumber!,
      ProfilePicture = user.ProfilePicture,
      CreatedAt = user.CreatedAt
    };
  }
}
