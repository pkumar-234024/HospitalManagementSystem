using System.ComponentModel.DataAnnotations;
namespace HospitalManagementSystem.UseCases.Authentication.Dtos;

public class LoginRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  public string IpAddress { get; set; } = string.Empty;

  [Required]
  public string UserAgent { get; set; } = string.Empty;
}
