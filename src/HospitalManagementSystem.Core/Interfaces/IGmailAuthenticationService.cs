using HospitalManagementSystem.Core.Model.User;
namespace HospitalManagementSystem.Core.Interfaces;public interface IGmailAuthenticationService
{
  Task<(bool Success, ApplicationUser User, string Message)> AuthenticateWithGoogleTokenAsync(string idToken);
  Task<bool> LinkGoogleAccountAsync(ApplicationUser user, string idToken);
  Task<bool> UnlinkGoogleAccountAsync(ApplicationUser user);
}
