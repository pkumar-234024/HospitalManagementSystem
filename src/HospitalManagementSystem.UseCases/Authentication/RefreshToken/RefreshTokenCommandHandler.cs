
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Dtos.Login;
using HospitalManagementSystem.UseCases.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly UserManager<ApplicationUser> _userManager;

  public RefreshTokenCommandHandler(
      IRefreshTokenService refreshTokenService,
      IJwtTokenService jwtTokenService,
      UserManager<ApplicationUser> userManager)
  {
    _refreshTokenService = refreshTokenService;
    _jwtTokenService = jwtTokenService;
    _userManager = userManager;
  }

  public async ValueTask<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
  {
    try
    {
      // This would be retrieved from the token claims
      var userId = Guid.Parse(request.RefreshToken.Split('.')[0]); // Simplified, use claims in real implementation

      // Validate refresh token
      var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken);

      if (!isValid)
      {
        return new LoginResponse
        {
          Success = false,
          Message = "Invalid or expired refresh token"
        };
      }

      var user = await _userManager.FindByIdAsync(userId.ToString());

      if (user == null || !user.IsActive)
      {
        return new LoginResponse
        {
          Success = false,
          Message = "User not found or inactive"
        };
      }

      // Revoke old token
      await _refreshTokenService.RevokeRefreshTokenAsync(userId, request.RefreshToken);

      // Get roles
      var roles = await _userManager.GetRolesAsync(user);

      // Generate new tokens
      var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
      var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
          userId.ToString(), request.IpAddress, request.UserAgent);

      return new LoginResponse
      {
        Success = true,
        Message = "Token refreshed successfully",
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
        Message = $"Token refresh failed: {ex.Message}"
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
