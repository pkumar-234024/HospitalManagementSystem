using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HospitalManagementSystem.UseCases.Authentication.ValidateToken;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class ValidateToken(IMediator mediator)
    : Endpoint<
        ValidateTokenRequest,
        Results<
            Ok<ValidateTokenResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(ValidateTokenRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Validate Token";
      s.Description = "Validates JWT token and returns token status.";

      s.ExampleRequest = new ValidateTokenRequest
      {
        Token = "your-jwt-token"
      };

      s.Responses[200] = "Token validation completed";
      s.Responses[400] = "Invalid request";
    });

    Description(builder => builder
        .Accepts<ValidateTokenRequest>("application/json")
        .Produces<ValidateTokenResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<ValidateTokenResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          ValidateTokenRequest request,
          CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Token))
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = "Token is required"
          });
    }

    var query = new ValidateTokenQuery
    {
      Token = request.Token
    };

    var isValid =
        await _mediator.Send(query, cancellationToken);

    return TypedResults.Ok(
        new ValidateTokenResponse
        {
          IsValid = isValid
        });
  }
}

public class ValidateTokenRequest
{
  public const string Route = "/auth/validate-token";

  [Required]
  public string Token { get; set; } = string.Empty;
}

public class ValidateTokenValidator
    : Validator<ValidateTokenRequest>
{
  public ValidateTokenValidator()
  {
    RuleFor(x => x.Token)
        .NotEmpty();
  }
}

public class ValidateTokenResponse
{
  public bool IsValid { get; set; }
}
