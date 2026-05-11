namespace HospitalManagementSystem.Core.Options;
public class GmailOptions
{
  public const string Section = "Gmail";

  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string RedirectUri { get; set; } = string.Empty;
  public string[] Scopes { get; set; } = new[] { "email", "profile" };
}
