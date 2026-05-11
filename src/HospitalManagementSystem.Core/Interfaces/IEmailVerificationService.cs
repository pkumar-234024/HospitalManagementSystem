using HospitalManagementSystem.Core.Model.User;
namespace HospitalManagementSystem.Core.Interfaces;
public interface IEmailVerificationService
{
  Task<bool> SendVerificationEmailAsync(ApplicationUser user);
  Task<bool> VerifyEmailAsync(ApplicationUser user, string token);
  Task<bool> ResendVerificationEmailAsync(string email);
  Task<bool> IsEmailVerifiedAsync(Guid userId);
}
