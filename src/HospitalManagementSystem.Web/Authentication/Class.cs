using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.UseCases.Authentication.GoogleAuth;
using HospitalManagementSystem.UseCases.Dtos.Login;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class GoogleAuth(
    IMediator mediator,
    IJwtTokenService jwtTokenService)
    : Endpoint<
        GoogleAuthRequest,
        Results<
            Ok<LoginResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  private readonly IMediator _mediator = mediator;
  private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

  public override void Configure()
  {
    Post(GoogleAuthRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Google Authentication";
      s.Description = "Authenticates user using Google ID token.";

      s.ExampleRequest = new GoogleAuthRequest
      {
        IdToken = "google-id-token"
      };

      s.Responses[200] = "Google authentication successful";
      s.Responses[400] = "Google authentication failed";
    });

    Description(builder => builder
        .Accepts<GoogleAuthRequest>("application/json")
        .Produces<LoginResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<LoginResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          GoogleAuthRequest request,
          CancellationToken cancellationToken)
  {
    var command = new GoogleAuthCommand
    {
      IdToken = request.IdToken,
      UserAgent = HttpContext.Request.Headers.UserAgent.ToString()
    };

    Result<LoginResponse> response =
        await _mediator.Send(command, cancellationToken);

    if (!response.IsSuccess)
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = response.Errors.FirstOrDefault()
                        ?? "Google authentication failed"
          });
    }

    // Set authentication cookies
    _jwtTokenService.SetAuthenticationCookies(
        response.Value.AccessToken,
        response.Value.RefreshToken);

    return TypedResults.Ok(response.Value);
  }
}

public class GoogleAuthRequest
{
  public const string Route = "/auth/google";

  [Required]
  public string IdToken { get; set; } = string.Empty;
}

public class GoogleAuthValidator
    : Validator<GoogleAuthRequest>
{
  public GoogleAuthValidator()
  {
    RuleFor(x => x.IdToken)
        .NotEmpty();
  }
}
