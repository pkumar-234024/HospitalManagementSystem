using HospitalManagementSystem.UseCases.Dtos.User;
namespace HospitalManagementSystem.UseCases.Dtos.Login;
public class LoginResponse
{
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public UserDto User { get; set; } = null!;
  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
  public DateTime AccessTokenExpires { get; set; }
  public DateTime RefreshTokenExpires { get; set; }
  public bool EmailVerified { get; set; }
  public bool IsAccountLocked { get; set; }
}
