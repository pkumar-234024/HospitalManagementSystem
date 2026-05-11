using System.ComponentModel.DataAnnotations;
namespace HospitalManagementSystem.UseCases.Authentication.Dtos;
public class ResetPasswordRequest
{
  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  public string Token { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")]
  public string NewPassword { get; set; } = string.Empty;

  [Required]
  [Compare("NewPassword")]
  public string ConfirmPassword { get; set; } = string.Empty;
}
