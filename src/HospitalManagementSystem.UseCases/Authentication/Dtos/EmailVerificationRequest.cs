using System.ComponentModel.DataAnnotations;
namespace HospitalManagementSystem.UseCases.Authentication.Dtos;
public class EmailVerificationRequest
{
  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  public string Token { get; set; } = string.Empty;
}
