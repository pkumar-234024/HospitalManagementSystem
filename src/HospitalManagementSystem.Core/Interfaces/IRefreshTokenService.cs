using HospitalManagementSystem.Core.Model.User;
namespace HospitalManagementSystem.Core.Interfaces;

public interface IRefreshTokenService
{
  Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string ipAddress, string userAgent);
  Task<bool> ValidateRefreshTokenAsync(Guid userId, string token);
  Task<RefreshToken> GetValidRefreshTokenAsync(Guid userId, string token);
  Task RevokeRefreshTokenAsync(Guid userId, string token, string reason = "Manual revocation");
  Task RevokeAllUserTokensAsync(Guid userId, string reason = "User logout");
}
