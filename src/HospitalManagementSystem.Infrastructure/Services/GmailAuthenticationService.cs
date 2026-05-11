using Google.Apis.Auth;
using HospitalManagementSystem.Core.Enum;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using HospitalManagementSystem.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Infrastructure.Services;

public class GmailAuthenticationService : IGmailAuthenticationService
{
  private readonly GmailOptions _gmailOptions;
  private readonly AppDbContext _dbContext;
  private readonly UserManager<ApplicationUser> _userManager;

  public GmailAuthenticationService(
      IOptions<GmailOptions> gmailOptions,
      AppDbContext dbContext,
      UserManager<ApplicationUser> userManager)
  {
    _gmailOptions = gmailOptions.Value;
    _dbContext = dbContext;
    _userManager = userManager;
  }

  public async Task<(bool Success, ApplicationUser User, string Message)> AuthenticateWithGoogleTokenAsync(string idToken)
  {
    try
    {
      var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

      var user = await _dbContext.Users
          .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

      if (user == null)
      {
        user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
          user = new ApplicationUser
          {
            Id = Guid.NewGuid().ToString(),
            UserName = payload.Email,
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            GoogleId = payload.Subject,
            EmailVerifiedAt = DateTime.UtcNow,
            EmailConfirmed = true,
            AuthProvider = AuthProvider.Google,
            GoogleConnectedAt = DateTime.UtcNow
          };

          await _userManager.CreateAsync(user);
        }
        else
        {
          user.GoogleId = payload.Subject;
          user.AuthProvider = AuthProvider.Google;
          user.GoogleConnectedAt = DateTime.UtcNow;
          await _userManager.UpdateAsync(user);
        }
      }

      return (true, user, "Google authentication successful");
    }
    catch (InvalidOperationException ex)
    {
      return (false, null!, $"Invalid Google token: {ex.Message}");
    }
    catch (Exception ex)
    {
      return (false, null!, $"Google authentication failed: {ex.Message}");
    }
  }

  public async Task<bool> LinkGoogleAccountAsync(ApplicationUser user, string idToken)
  {
    try
    {
      var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

      user.GoogleId = payload.Subject;
      user.GoogleConnectedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(user);
      return result.Succeeded;
    }
    catch
    {
      return false;
    }
  }

  public async Task<bool> UnlinkGoogleAccountAsync(ApplicationUser user)
  {
    try
    {
      user.GoogleId = null!;
      user.GoogleConnectedAt = null;

      var result = await _userManager.UpdateAsync(user);
      return result.Succeeded;
    }
    catch
    {
      return false;
    }
  }
}
