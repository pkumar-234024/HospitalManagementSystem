namespace HospitalManagementSystem.Core.Model.User;
public class UserEmail: IAggregateRoot
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Email { get; set; } = string.Empty;
  public bool IsPrimary { get; set; }
  public bool IsVerified { get; set; }
  public DateTime? VerifiedAt { get; set; }
  public string VerificationToken { get; set; } = string.Empty;
  public DateTime? VerificationTokenExpiryAt { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation
  public virtual ApplicationUser User { get; set; } = null!;
}
