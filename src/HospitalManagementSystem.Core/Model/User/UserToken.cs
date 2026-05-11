namespace HospitalManagementSystem.Core.Model.User;
public class UserToken
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string AccessToken { get; set; } =string.Empty;
  public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
  public DateTime ExpiresAt { get; set; }
  public bool IsRevoked { get; set; }
  public string RevokeReason { get; set; } = string.Empty;
  public DateTime? RevokedAt { get; set; }

  // Navigation
  public virtual ApplicationUser User { get; set; } = null!;
}
