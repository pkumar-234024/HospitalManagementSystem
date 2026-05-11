using HospitalManagementSystem.UseCases.Authentication.Dtos;
namespace HospitalManagementSystem.UseCases.Authentication.GetAuthStatus;

public class GetAuthStatusQuery : IQuery<AuthStatusResponse>
{
  public string UserId { get; set; } = string.Empty;
}
