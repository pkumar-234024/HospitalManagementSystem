using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Infrastructure.Services;

public class EmailVerificationService : IEmailVerificationService
{
  private readonly AppDbContext _dbContext;
  private readonly IEmailService _emailService;
  private readonly EmailOptions _emailOptions;
  private readonly UserManager<ApplicationUser> _userManager;

  public EmailVerificationService(
      AppDbContext dbContext,
      IEmailService emailService,
      IOptions<EmailOptions> emailOptions,
      UserManager<ApplicationUser> userManager)
  {
    _dbContext = dbContext;
    _emailService = emailService;
    _emailOptions = emailOptions.Value;
    _userManager = userManager;
  }

  public async Task<bool> SendVerificationEmailAsync(ApplicationUser user)
  {
    try
    {
      var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

      user.EmailVerificationToken = token;
      _dbContext.Users.Update(user);
      await _dbContext.SaveChangesAsync();

      var verificationLink = $"https://yourdomain.com/api/auth/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

      await _emailService.SendEmailAsync(
          user.Email!,
          "Email Verification",
          $@"
                        <h2>Welcome to Hospital Management System</h2>
                        <p>Please verify your email by clicking the link below:</p>
                        <a href='{verificationLink}'>Verify Email</a>
                        <p>This link will expire in 24 hours.</p>
                    "
      );

      return true;
    }
    catch
    {
      return false;
    }
  }

  public async Task<bool> VerifyEmailAsync(ApplicationUser user, string token)
  {
    try
    {
      if (user.EmailVerificationToken != token)
        return false;

      user.EmailVerifiedAt = DateTime.UtcNow;
      user.EmailConfirmed = true;
      user.EmailVerificationToken = null!;

      _dbContext.Users.Update(user);
      await _dbContext.SaveChangesAsync();

      return true;
    }
    catch
    {
      return false;
    }
  }

  public async Task<bool> ResendVerificationEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
      return false;

    if (user.EmailVerifiedAt.HasValue)
      return false;

    return await SendVerificationEmailAsync(user);
  }

  public async Task<bool> IsEmailVerifiedAsync(Guid userId)
  {
    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());
    return user?.EmailVerifiedAt.HasValue ?? false;
  }
}
