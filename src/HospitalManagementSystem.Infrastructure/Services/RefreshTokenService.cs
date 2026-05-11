using System.Security.Cryptography;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using HospitalManagementSystem.Infrastructure.Data;
namespace HospitalManagementSystem.Infrastructure.Services;
public class RefreshTokenService : IRefreshTokenService
{
  private readonly JwtOptions _jwtOptions;
  private readonly AppDbContext _dbContext;

  public RefreshTokenService(IOptions<JwtOptions> jwtOptions, AppDbContext dbContext)
  {
    _jwtOptions = jwtOptions.Value;
    _dbContext = dbContext;
  }

  public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress, string userAgent)
  {
    var refreshToken = new RefreshToken
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Token = GenerateRandomToken(),
      ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
      IpAddress = ipAddress,
      UserAgent = userAgent
    };

    _dbContext.RefreshTokens.Add(refreshToken);
    await _dbContext.SaveChangesAsync();

    return refreshToken;
  }

  public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string token)
  {
    var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

    return refreshToken != null && refreshToken.IsActive;
  }

  public async Task<RefreshToken> GetValidRefreshTokenAsync(Guid userId, string token)
  {
    var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

    if (refreshToken?.IsActive == true)
      return refreshToken;

    return null!;
  }

  public async Task RevokeRefreshTokenAsync(Guid userId, string token, string reason = "Manual revocation")
  {
    var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

    if (refreshToken != null)
    {
      refreshToken.RevokedAt = DateTime.UtcNow;
      _dbContext.RefreshTokens.Update(refreshToken);
      await _dbContext.SaveChangesAsync();
    }
  }

  public async Task RevokeAllUserTokensAsync(Guid userId, string reason = "User logout")
  {
    var tokens = await _dbContext.RefreshTokens
        .Where(rt => rt.UserId == userId && !rt.IsRevoked)
        .ToListAsync();

    foreach (var token in tokens)
    {
      token.RevokedAt = DateTime.UtcNow;
    }

    _dbContext.RefreshTokens.UpdateRange(tokens);
    await _dbContext.SaveChangesAsync();
  }

  private string GenerateRandomToken()
  {
    var randomNumber = new byte[64];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }
  }

  public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string ipAddress, string userAgent)
  {
    var refreshToken = new RefreshToken
    {
      Id = Guid.NewGuid(),
      UserId =  Guid.Parse(userId),
      Token = GenerateRandomToken(),
      ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
      IpAddress = ipAddress,
      UserAgent = userAgent
    };

    _dbContext.RefreshTokens.Add(refreshToken);
    await _dbContext.SaveChangesAsync();

    return refreshToken;
  }
}

