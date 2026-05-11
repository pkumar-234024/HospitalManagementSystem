namespace HospitalManagementSystem.UseCases.Authentication.VerifyEmail;
public class VerifyEmailCommand : ICommand<bool>
{
  public string UserId { get; set; } = string.Empty;
  public string Token { get; set; }= string.Empty;
}
