namespace HospitalManagementSystem.Core.Interfaces;
public interface IEmailService
{
  Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);
  Task<bool> SendBulkEmailAsync(string[] emails, string subject, string htmlMessage);
}
