namespace HospitalManagementSystem.UseCases.Authentication.ValidateToken;
public class ValidateTokenQuery : IQuery<bool>
{
  public string Token { get; set; } = string.Empty;
}
