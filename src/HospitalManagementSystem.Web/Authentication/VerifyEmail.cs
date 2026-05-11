using System.ComponentModel.DataAnnotations;
using FluentValidation;
using global::HospitalManagementSystem.UseCases.Authentication.VerifyEmail;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class VerifyEmail(IMediator mediator)
    : Endpoint<
        EmailVerificationRequest,
        Results<
            Ok<MessageResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(EmailVerificationRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Verify Email";
      s.Description = "Verifies user email using verification token.";

      s.ExampleRequest = new EmailVerificationRequest
      {
        UserId = Guid.NewGuid(),
        Token = "email-verification-token"
      };

      s.Responses[200] = "Email verified successfully";
      s.Responses[400] = "Email verification failed";
    });

    Description(builder => builder
        .Accepts<EmailVerificationRequest>("application/json")
        .Produces<MessageResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          EmailVerificationRequest request,
          CancellationToken cancellationToken)
  {
    var command = new VerifyEmailCommand
    {
      UserId = request.UserId.ToString(),
      Token = request.Token
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result)
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = "Email verification failed"
          });
    }

    return TypedResults.Ok(
        new MessageResponse
        {
          Message = "Email verified successfully"
        });
  }
}

public class EmailVerificationRequest
{
  public const string Route = "/auth/verify-email";

  [Required]
  public Guid UserId { get; set; }

  [Required]
  public string Token { get; set; } = string.Empty;
}

public class EmailVerificationValidator
    : Validator<EmailVerificationRequest>
{
  public EmailVerificationValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty();

    RuleFor(x => x.Token)
        .NotEmpty();
  }
}

public class MessageResponse
{
  public string Message { get; set; } = string.Empty;
}
