using HospitalManagementSystem.Core.Interfaces;

namespace HospitalManagementSystem.UseCases.Authentication.ValidateToken;

public class ValidateTokenQueryHandler : IQueryHandler<ValidateTokenQuery, bool>
{
  private readonly IJwtTokenService _jwtTokenService;

  public ValidateTokenQueryHandler(IJwtTokenService jwtTokenService)
  {
    _jwtTokenService = jwtTokenService;
  }

  public async ValueTask<bool> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
  {
    return await Task.FromResult(_jwtTokenService.ValidateToken(request.Token));
  }
}
