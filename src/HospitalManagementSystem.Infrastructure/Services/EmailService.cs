using System.Net;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Options;
namespace HospitalManagementSystem.Infrastructure.Services;
public class EmailService : IEmailService
{
  private readonly EmailOptions _emailOptions;

  public EmailService(IOptions<EmailOptions> emailOptions)
  {
    _emailOptions = emailOptions.Value;
  }

  public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
  {
    try
    {
      using (var client = new System.Net.Mail.SmtpClient(_emailOptions.SmtpServer, _emailOptions.SmtpPort))
      {
        client.EnableSsl = _emailOptions.EnableSsl;
        client.Credentials = new NetworkCredential(_emailOptions.SenderEmail, _emailOptions.SenderPassword);

        using (var mailMessage = new MailMessage(_emailOptions.SenderEmail, email))
        {
          mailMessage.Subject = subject;
          mailMessage.Body = htmlMessage;
          mailMessage.IsBodyHtml = true;

          await client.SendMailAsync(mailMessage);
        }
      }

      return true;
    }
    catch (Exception ex)
    {
      // Log exception
      Console.WriteLine(ex.ToString());
      return false;
    }
  }

  public async Task<bool> SendBulkEmailAsync(string[] emails, string subject, string htmlMessage)
  {
    try
    {
      using (var client = new System.Net.Mail.SmtpClient(_emailOptions.SmtpServer, _emailOptions.SmtpPort))
      {
        client.EnableSsl = _emailOptions.EnableSsl;
        client.Credentials = new NetworkCredential(_emailOptions.SenderEmail, _emailOptions.SenderPassword);

        foreach (var email in emails)
        {
          using (var mailMessage = new MailMessage(new MailAddress(_emailOptions.SenderEmail), new MailAddress(email)))
          {
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;

            await client.SendMailAsync(mailMessage);
          }
        }
      }

      return true;
    }
    catch (Exception ex)
    {
      // Log exception
      Console.WriteLine(ex.ToString());
      return false;
    }
  }
}
