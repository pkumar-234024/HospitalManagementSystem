using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Dtos.Login;
using HospitalManagementSystem.UseCases.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IRefreshTokenService _refreshTokenService;
  private const int MaxFailedAttempts = 5;
  private const int LockoutDurationMinutes = 30;

  public LoginCommandHandler(
      UserManager<ApplicationUser> userManager,
      IJwtTokenService jwtTokenService,
      IRefreshTokenService refreshTokenService)
  {
    _userManager = userManager;
    _jwtTokenService = jwtTokenService;
    _refreshTokenService = refreshTokenService;
  }

  public async ValueTask<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null)
      {
        return new LoginResponse
        {
          Success = false,
          Message = "Entered the Incorect Email!"
        };
      }

      // Check if account is locked
      if (user.IsAccountLocked && user.LastLockoutDate.HasValue)
      {
        var lockoutExpiry = user.LastLockoutDate.Value.AddMinutes(LockoutDurationMinutes);
        if (DateTime.UtcNow < lockoutExpiry)
        {
          return new LoginResponse
          {
            Success = false,
            Message = $"Account is locked. Try again after {lockoutExpiry:yyyy-MM-dd HH:mm:ss} UTC",
            IsAccountLocked = true
          };
        }
        else
        {
          // Unlock account after lockout period
          user.IsAccountLocked = false;
          user.AccessFailedCount = 0;
          user.LastLockoutDate = null;
          await _userManager.UpdateAsync(user);
        }
      }

      // Verify password
      var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

      if (!passwordValid)
      {
        user.AccessFailedCount++;

        if (user.AccessFailedCount >= MaxFailedAttempts)
        {
          user.IsAccountLocked = true;
          user.LastLockoutDate = DateTime.UtcNow;
        }

        await _userManager.UpdateAsync(user);

        return new LoginResponse
        {
          Success = false,
          Message = $"InCorrect Passsword. Attempts remaining: {MaxFailedAttempts - user.AccessFailedCount}",
          IsAccountLocked = user.IsAccountLocked
        };
      }

      // Reset failed attempts on successful login
      user.AccessFailedCount = 0;
      user.IsAccountLocked = false;
      user.LastLockoutDate = null;
      await _userManager.UpdateAsync(user);

      // Get user roles
      var roles = await _userManager.GetRolesAsync(user);

      // Generate tokens
      var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
      var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
          user.Id.ToString(), request.IpAddress, request.UserAgent);

      var response = new LoginResponse
      {
        Success = true,
        Message = "Login successful",
        User = MapUserToDto(user),
        AccessToken = accessToken,
        RefreshToken = refreshToken.Token,
        AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpires = refreshToken.ExpiresAt,
        EmailVerified = user.EmailVerifiedAt.HasValue,
        IsAccountLocked = false
      };

      return response;
    }
    catch (Exception ex)
    {
      return new LoginResponse
      {
        Success = false,
        Message = $"Login failed: {ex.Message}"
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
      HospitalId = user.HospitalId,
      HospitalName = string.Empty,
      CreatedAt = user.CreatedAt
    };
  }
}
