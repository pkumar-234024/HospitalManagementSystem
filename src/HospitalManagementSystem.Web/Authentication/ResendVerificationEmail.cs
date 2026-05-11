using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class ResendVerificationEmail
    : Endpoint<
        ResendVerificationEmailRequest,
        Results<
            Ok<MessageResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  public override void Configure()
  {
    Post(ResendVerificationEmailRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Resend Verification Email";
      s.Description = "Sends a new email verification link to the user.";

      s.ExampleRequest = new ResendVerificationEmailRequest
      {
        Email = "john@example.com"
      };

      s.Responses[200] = "Verification email sent successfully";
      s.Responses[400] = "Invalid email address";
    });

    Description(builder => builder
        .Accepts<ResendVerificationEmailRequest>("application/json")
        .Produces<MessageResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          ResendVerificationEmailRequest request,
          CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = "Email is required"
          });
    }

    // TODO:
    // Call your email verification service here
    // Example:
    // await _emailVerificationService
    //     .ResendVerificationEmailAsync(request.Email);

    await Task.CompletedTask;

    return TypedResults.Ok(
        new MessageResponse
        {
          Message = "Verification email sent"
        });
  }
}

public class ResendVerificationEmailRequest
{
  public const string Route = "/auth/resend-verification-email";

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
}

public class ResendVerificationEmailValidator
    : Validator<ResendVerificationEmailRequest>
{
  public ResendVerificationEmailValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();
  }
}
