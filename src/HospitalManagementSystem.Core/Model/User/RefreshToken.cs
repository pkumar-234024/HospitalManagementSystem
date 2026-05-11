namespace HospitalManagementSystem.Core.Model.User;
public class RefreshToken:IAggregateRoot
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Token { get; set; } = string.Empty;
  public string JwtTokenId { get; set; } = string.Empty;// Tie to specific JWT
  public DateTime ExpiresAt { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? RevokedAt { get; set; }
  public bool IsRevoked => RevokedAt != null;
  public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
  public bool IsActive => !IsRevoked && !IsExpired;
  public string IpAddress { get; set; } = string.Empty;
  public string UserAgent { get; set; } = string.Empty;

  // Navigation
  public virtual ApplicationUser User { get; set; } = null!;
}
