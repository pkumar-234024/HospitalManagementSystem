namespace HospitalManagementSystem.UseCases.Authentication.AdminResetPassword;

public class AdminResetPasswordCommand : ICommand<bool>
{
  public string UserId { get; set; } = string.Empty;
  public string NewPassword { get; set; } = string.Empty;
  public string AdminId { get; set; } = string.Empty; // Admin performing the action
}
