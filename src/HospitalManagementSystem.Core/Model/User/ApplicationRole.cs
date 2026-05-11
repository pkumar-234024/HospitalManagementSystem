using Microsoft.AspNetCore.Identity;
namespace HospitalManagementSystem.Core.Model.User;
public class ApplicationRole :IdentityRole
{
  public string Role { get; set; } = string.Empty;
}
