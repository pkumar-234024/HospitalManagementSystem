namespace HospitalManagementSystem.Core.Options;
public class EmailOptions
{
  public const string Section = "Email";
  public string SmtpServer { get; set; } = string.Empty;
  public int SmtpPort { get; set; }
  public string SenderEmail { get; set; } = string.Empty;
  public string SenderPassword { get; set; } = string.Empty;
  public bool EnableSsl { get; set; } = true;
  public int EmailVerificationTokenExpirationMinutes { get; set; } = 24 * 60; // 24 hours
  public int PasswordResetTokenExpirationMinutes { get; set; } = 30;
}
