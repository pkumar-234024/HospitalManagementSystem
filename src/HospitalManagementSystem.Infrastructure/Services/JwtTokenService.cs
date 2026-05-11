using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
  private readonly JwtOptions _jwtOptions;
  private readonly IEmailService _emailService;
  private const string AccessTokenCookie = "accessToken";
  private const string RefreshTokenCookie = "refreshToken";
  private readonly IHttpContextAccessor _httpContextAccessor;

  public JwtTokenService(IOptions<JwtOptions> jwtOptions, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
  {
    _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
    _emailService = emailService;
    _httpContextAccessor = httpContextAccessor;
  }

  public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles = null!)
  {
    if (user == null)
      throw new ArgumentNullException(nameof(user));

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName!} {user.LastName!}"),
                new Claim("EmailVerified", user.EmailVerifiedAt.HasValue.ToString()),
                new Claim("IsAccountLocked", user.IsAccountLocked.ToString()),
            };

    if (roles != null)
    {
      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
    }

    var token = new JwtSecurityToken(
        issuer: _jwtOptions.Issuer,
        audience: _jwtOptions.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string GenerateEmailVerificationToken(ApplicationUser user)
  {
    var randomToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
    return randomToken;
  }

  public string GeneratePasswordResetToken(ApplicationUser user)
  {
    var randomToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
    return randomToken;
  }

  public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
  {
    try
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = _jwtOptions.ValidateAudience,
        ValidAudience = _jwtOptions.Audience,
        ValidateIssuer = _jwtOptions.ValidateIssuer,
        ValidIssuer = _jwtOptions.Issuer,
        ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
        ValidateLifetime = false
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
      return principal;
    }
    catch
    {
      return null!;
    }
  }

  public void SetAuthenticationCookies(string accessToken, string refreshToken)
  {
    try
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
      };
      var context = _httpContextAccessor.HttpContext;
      if (context == null)
      {
        throw new Exception("HttpContext is null");
      }
      context.Response.Cookies.Append(AccessTokenCookie, accessToken, cookieOptions);

      var refreshCookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(7)
      };
      context.Response.Cookies.Append(RefreshTokenCookie, refreshToken, refreshCookieOptions);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString()); 
    }
  }

  public bool ValidateToken(string token)
  {
    try
    {
      var principal = GetPrincipalFromExpiredToken(token);
      return principal != null;
    }
    catch
    {
      return false;
    }
  }
}
