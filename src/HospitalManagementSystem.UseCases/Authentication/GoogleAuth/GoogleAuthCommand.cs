using HospitalManagementSystem.UseCases.Dtos.Login;
namespace HospitalManagementSystem.UseCases.Authentication.GoogleAuth;
public class GoogleAuthCommand : ICommand<LoginResponse>
{
  public string IdToken { get; set; } = string.Empty;
  public string IpAddress { get; set; } = string.Empty;
  public string UserAgent { get; set; } = string.Empty;
}
