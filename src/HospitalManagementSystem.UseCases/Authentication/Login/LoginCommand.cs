using HospitalManagementSystem.UseCases.Dtos.Login;
namespace HospitalManagementSystem.UseCases.Authentication.Login;
public record LoginCommand : ICommand<Result<LoginResponse>>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string IpAddress { get; set; } = string.Empty;
  public string UserAgent { get; set; } = string.Empty;
}
