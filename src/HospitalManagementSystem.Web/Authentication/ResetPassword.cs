using System.ComponentModel.DataAnnotations;
using FluentValidation;
using global::HospitalManagementSystem.UseCases.Authentication.ResetPassword;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class ResetPassword(IMediator mediator)
    : Endpoint<
        ResetPasswordRequest,
        Results<
            Ok<MessageResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(ResetPasswordRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Reset Password";
      s.Description = "Resets user password using reset token.";

      s.ExampleRequest = new ResetPasswordRequest
      {
        UserId = Guid.NewGuid(),
        Token = "reset-password-token",
        NewPassword = "NewPassword@123",
        ConfirmPassword = "NewPassword@123"
      };

      s.Responses[200] = "Password reset successfully";
      s.Responses[400] = "Password reset failed";
    });

    Description(builder => builder
        .Accepts<ResetPasswordRequest>("application/json")
        .Produces<MessageResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json"));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>>>
      ExecuteAsync(
          ResetPasswordRequest request,
          CancellationToken cancellationToken)
  {
    var command = new ResetPasswordCommand
    {
      UserId = request.UserId.ToString(),
      Token = request.Token,
      NewPassword = request.NewPassword,
      ConfirmPassword = request.ConfirmPassword
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result)
    {
      return TypedResults.BadRequest(
          new MessageResponse
          {
            Message = "Password reset failed"
          });
    }

    return TypedResults.Ok(
        new MessageResponse
        {
          Message = "Password reset successfully"
        });
  }
}

public class ResetPasswordRequest
{
  public const string Route = "/auth/reset-password";

  [Required]
  public Guid UserId { get; set; }

  [Required]
  public string Token { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string NewPassword { get; set; } = string.Empty;

  [Required]
  [Compare(nameof(NewPassword))]
  public string ConfirmPassword { get; set; } = string.Empty;
}

public class ResetPasswordValidator
    : Validator<ResetPasswordRequest>
{
  public ResetPasswordValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty();

    RuleFor(x => x.Token)
        .NotEmpty();

    RuleFor(x => x.NewPassword)
        .NotEmpty()
        .MinimumLength(6);

    RuleFor(x => x.ConfirmPassword)
        .NotEmpty()
        .Equal(x => x.NewPassword)
        .WithMessage("Passwords do not match.");
  }
}
