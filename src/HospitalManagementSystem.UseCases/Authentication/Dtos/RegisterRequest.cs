using System.ComponentModel.DataAnnotations;
namespace HospitalManagementSystem.UseCases.Authentication.Dtos;
public class RegisterRequest
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;  

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
      ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
  public string Password { get; set; } = string.Empty;
  [Required]
  [Compare("Password")]
  public string ConfirmPassword { get; set; } = string.Empty;

  [Phone]
  public string PhoneNumber { get; set; } = string.Empty;

  [Required]
  public string IpAddress { get; set; }= string.Empty;

  [Required]
  public string UserAgent { get; set; }= string.Empty;
}
