using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Dtos.Login;
using HospitalManagementSystem.UseCases.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Authentication.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, LoginResponse>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IEmailVerificationService _emailVerificationService;

  public RegisterCommandHandler(
      UserManager<ApplicationUser> userManager,
      IJwtTokenService jwtTokenService,
      IRefreshTokenService refreshTokenService,
      IEmailVerificationService emailVerificationService)
  {
    _userManager = userManager;
    _jwtTokenService = jwtTokenService;
    _refreshTokenService = refreshTokenService;
    _emailVerificationService = emailVerificationService;
  }

  public async ValueTask<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
  {
    try
    {
      // Check if user exists
      var existingUser = await _userManager.FindByEmailAsync(request.Email);
      if (existingUser != null)
      {
        return new LoginResponse
        {
          Success = false,
          Message = "Email already registered"
        };
      }

      // Create new user
      var user = new ApplicationUser
      {
        Id = Guid.NewGuid().ToString(),
        UserName = request.Email,
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName,
        PhoneNumber = request.PhoneNumber,
        EmailConfirmed = false,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
      };

      var result = await _userManager.CreateAsync(user, request.Password);

      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return new LoginResponse
        {
          Success = false,
          Message = $"Registration failed: {errors}"
        };
      }

      // Assign default role
      await _userManager.AddToRoleAsync(user, "Patient");

      // Send verification email
      await _emailVerificationService.SendVerificationEmailAsync(user);

      // Generate tokens
      var accessToken = _jwtTokenService.GenerateAccessToken(user, new List<string> { "Patient" });
      var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(
          user.Id, request.IpAddress, request.UserAgent);

      return new LoginResponse
      {
        Success = true,
        Message = "Registration successful. Please verify your email.",
        User = MapUserToDto(user),
        AccessToken = accessToken,
        RefreshToken = refreshToken.Token,
        AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpires = refreshToken.ExpiresAt,
        EmailVerified = false
      };
    }
    catch (Exception ex)
    {
      return new LoginResponse
      {
        Success = false,
        Message = $"Registration failed: {ex.Message}"
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
