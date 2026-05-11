namespace HospitalManagementSystem.Core.Options;
public class JwtOptions
{
  public const string Section = "Jwt";

  public string SecretKey { get; set; } =default!;
  public string Issuer { get; set; } = default!;
  public string Audience { get; set; } = default!;
  public int AccessTokenExpirationMinutes { get; set; } = 15;
  public int RefreshTokenExpirationDays { get; set; } = 7;
  public bool ValidateIssuerSigningKey { get; set; } = true;
  public bool ValidateIssuer { get; set; } = true;
  public bool ValidateAudience { get; set; } = true;
  public bool ValidateLifetime { get; set; } = true;
}
