namespace HospitalManagementSystem.UseCases.Authentication.ForgotPassword;

public class ForgotPasswordCommand : ICommand<bool>
{
  public string Email { get; set; }=string.Empty;
}
