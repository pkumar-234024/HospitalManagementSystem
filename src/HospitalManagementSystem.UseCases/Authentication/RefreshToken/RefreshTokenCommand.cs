using HospitalManagementSystem.UseCases.Dtos.Login;
namespace HospitalManagementSystem.UseCases.Authentication.RefreshToken;

public class RefreshTokenCommand : ICommand<LoginResponse>
{
  public string RefreshToken { get; set; } = string.Empty;
  public string IpAddress { get; set; } = string.Empty;
  public string UserAgent { get; set; } = string.Empty;
}
