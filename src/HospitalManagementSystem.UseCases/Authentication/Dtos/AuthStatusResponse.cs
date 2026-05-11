namespace HospitalManagementSystem.UseCases.Authentication.Dtos;
public class AuthStatusResponse
{
  public bool IsAuthenticated { get; set; }
  public string UserId { get; set; }= string.Empty;
  public string Email { get; set; } = string.Empty;
  public List<string> Roles { get; set; } = new();
  public bool EmailVerified { get; set; }
  public bool IsAccountLocked { get; set; }
  public int AccessFailedCount { get; set; }
  public DateTime TokenExpires { get; set; }
}
