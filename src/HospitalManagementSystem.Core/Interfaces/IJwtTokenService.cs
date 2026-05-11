using System.Security.Claims;
using HospitalManagementSystem.Core.Model.User;

namespace HospitalManagementSystem.Core.Interfaces;

public interface IJwtTokenService
{
  string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles = null!);
  string GenerateEmailVerificationToken(ApplicationUser user);
  string GeneratePasswordResetToken(ApplicationUser user);
  ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
  bool ValidateToken(string token);
  void SetAuthenticationCookies(string accessToken, string refreshToken);
}
