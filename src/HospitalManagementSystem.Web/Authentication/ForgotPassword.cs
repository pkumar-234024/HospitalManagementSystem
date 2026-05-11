using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HospitalManagementSystem.UseCases.Authentication.ForgotPassword;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class ForgotPassword(IMediator mediator)
    : Endpoint<
        ForgotPasswordRequest,
        Results<
            Ok<MessageResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(ForgotPasswordRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Forgot Password";
      s.Description = "Sends password reset email to the user.";

      s.ExampleRequest = new ForgotPasswordRequest
      {
        Email = "john@example.com"
      };

      s.Responses[200] = "Password reset email sent successfully";
      s.Responses[400] = "Failed to process password reset request";
    });

    Description(builder => builder
        .Accepts<ForgotPasswordRequest>("application/json")
        .Produces<MessageResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          ForgotPasswordRequest request,
          CancellationToken cancellationToken)
  {
    var command = new ForgotPasswordCommand
    {
      Email = request.Email
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result)
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = "Failed to process password reset request"
          });
    }

    return TypedResults.Ok(
        new MessageResponse
        {
          Message = "Password reset email sent successfully"
        });
  }
}

public class ForgotPasswordRequest
{
  public const string Route = "/auth/forgot-password";

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordValidator
    : Validator<ForgotPasswordRequest>
{
  public ForgotPasswordValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();
  }
}
