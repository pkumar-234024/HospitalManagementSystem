using FluentValidation;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.UseCases.Authentication.RefreshToken;
using HospitalManagementSystem.UseCases.Dtos.Login;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class RefreshToken(
    IMediator mediator,
    IJwtTokenService jwtTokenService)
    : Endpoint<
        RefreshTokenRequest,
        Results<
            Ok<LoginResponse>,
            ValidationProblem,
            UnauthorizedHttpResult>>
{
  private readonly IMediator _mediator = mediator;
  private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

  public override void Configure()
  {
    Post(RefreshTokenRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Refresh Access Token";
      s.Description = "Refreshes access token using a valid refresh token.";

      s.ExampleRequest = new RefreshTokenRequest
      {
        RefreshToken = "your-refresh-token"
      };

      s.Responses[200] = "Token refreshed successfully";
      s.Responses[401] = "Invalid or expired refresh token";
    });

    Description(builder => builder
        .Accepts<RefreshTokenRequest>("application/json")
        .Produces<LoginResponse>(200, "application/json")
        .Produces(401));
  }

  public override async Task<
      Results<
          Ok<LoginResponse>,
          ValidationProblem,
          UnauthorizedHttpResult>>
      ExecuteAsync(
          RefreshTokenRequest request,
          CancellationToken cancellationToken)
  {
    var refreshToken =
        request.RefreshToken
        ?? HttpContext.Request.Cookies["refreshToken"];

    var command = new RefreshTokenCommand
    {
      RefreshToken = refreshToken!,
    };

    Result<LoginResponse> response =
        await _mediator.Send(command, cancellationToken);

    if (!response.IsSuccess)
    {
      return TypedResults.Unauthorized();
    }

    // Update authentication cookies
    _jwtTokenService.SetAuthenticationCookies(
        response.Value.AccessToken,
        response.Value.RefreshToken);

    return TypedResults.Ok(response.Value);
  }
}

public class RefreshTokenRequest
{
  public const string Route = "/auth/refresh-token";

  public string? RefreshToken { get; set; }
}

public class RefreshTokenValidator : Validator<RefreshTokenRequest>
{
  public RefreshTokenValidator()
  {
    RuleFor(x => x.RefreshToken)
        .NotEmpty()
        .When(x => string.IsNullOrWhiteSpace(x.RefreshToken))
        .WithMessage("Refresh token is required.");
  }
}
