using HospitalManagementSystem.Core.Interfaces;

namespace HospitalManagementSystem.Web.Middleware;

public class JwtMiddleware
{
  private readonly RequestDelegate _next;

  public JwtMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IJwtTokenService jwtTokenService)
  {
    var token = ExtractTokenFromRequest(context);

    if (!string.IsNullOrEmpty(token))
    {
      context.Items["Token"] = token;

      // Try to validate token
      if (jwtTokenService.ValidateToken(token))
      {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(token);
        if (principal != null)
        {
          context.User = principal;
        }
      }
    }

    await _next(context);
  }

  private string ExtractTokenFromRequest(HttpContext context)
  {
    // Check Authorization header
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (authHeader.StartsWith("Bearer "))
    {
      return authHeader.Substring("Bearer ".Length).Trim();
    }

    // Check cookies
    if (context.Request.Cookies.TryGetValue("accessToken", out var cookieToken))
    {
      return cookieToken;
    }

    return null!;
  }
}
