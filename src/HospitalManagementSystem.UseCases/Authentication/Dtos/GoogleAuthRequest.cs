using System.ComponentModel.DataAnnotations;
namespace HospitalManagementSystem.UseCases.Authentication.Dtos;
public class GoogleAuthRequest
{
  [Required]
  public string IdToken { get; set; } = string.Empty;
  [Required]
  public string IpAddress { get; set; } = string.Empty;
  [Required]
  public string UserAgent { get; set; } = string.Empty;
}
