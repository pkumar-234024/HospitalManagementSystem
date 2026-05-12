using HospitalManagementSystem.Core.Enum;
using Microsoft.AspNetCore.Identity;
namespace HospitalManagementSystem.Core.Model.User;
public class ApplicationUser : IdentityUser
{

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public int UserId { get; set; }
  public string ProfilePicture { get; set; } = string.Empty;
  public Guid? HospitalId { get; set; }

  // Email Verification
  public DateTime? EmailVerifiedAt { get; set; }
  public string EmailVerificationToken { get; set; } = string.Empty;

  // Account Lockout
  public DateTime? LastLockoutDate { get; set; }
  public bool IsAccountLocked { get; set; }

  // OAuth
  public string GoogleId { get; set; } = string.Empty;
  public DateTime? GoogleConnectedAt { get; set; }
  public AuthProvider AuthProvider { get; set; }

  // Password Management
  public DateTime LastPasswordChangedAt { get; set; }
  public string PasswordResetToken { get; set; } = string.Empty;
  public DateTime? PasswordResetTokenExpiryAt { get; set; }
  public int PasswordChangeCount { get; set; }

  // Navigation
  public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
  public ICollection<UserEmail> UserEmails { get; set; } = new List<UserEmail>();
  public ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public bool IsActive { get; set; } = true;
}
