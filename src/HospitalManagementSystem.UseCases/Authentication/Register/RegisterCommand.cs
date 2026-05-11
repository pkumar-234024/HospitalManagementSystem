using HospitalManagementSystem.UseCases.Dtos.Login;
namespace HospitalManagementSystem.UseCases.Authentication.Register;
public class RegisterCommand : ICommand<LoginResponse>
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string ConfirmPassword { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string IpAddress { get; set; } = string.Empty;
  public string UserAgent { get; set; } = string.Empty;
}
