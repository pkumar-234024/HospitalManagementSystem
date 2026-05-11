using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FluentValidation;
using HospitalManagementSystem.UseCases.Authentication.AdminResetPassword;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class AdminResetPassword(IMediator mediator)
    : Endpoint<
        AdminResetPasswordRequest,
        Results<
            Ok<MessageResponse>,
            ValidationProblem,
            BadRequest<MessageResponse>,
            ForbidHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AdminResetPasswordRequest.Route);

    Roles("Admin");

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Admin Reset Password";
      s.Description = "Allows admin to reset a user's password.";

      s.ExampleRequest = new AdminResetPasswordRequest
      {
        UserId = "user-id",
        NewPassword = "Admin@123"
      };

      s.Responses[200] = "Password reset successfully";
      s.Responses[400] = "Password reset failed";
      s.Responses[403] = "Forbidden";
    });

    Description(builder => builder
        .Accepts<AdminResetPasswordRequest>("application/json")
        .Produces<MessageResponse>(200, "application/json")
        .Produces<MessageResponse>(400, "application/json")
        .Produces(403));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          ValidationProblem,
          BadRequest<MessageResponse>,
          ForbidHttpResult>>
      ExecuteAsync(
          AdminResetPasswordRequest request,
          CancellationToken cancellationToken)
  {
    var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(adminId))
    {
      return TypedResults.Forbid();
    }

    var command = new AdminResetPasswordCommand
    {
      UserId = request.UserId,
      NewPassword = request.NewPassword,
      AdminId = adminId
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

public class AdminResetPasswordRequest
{
  public const string Route = "/auth/admin/reset-password/{userId}";

  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string NewPassword { get; set; } = string.Empty;
}

public class AdminResetPasswordValidator
    : Validator<AdminResetPasswordRequest>
{
  public AdminResetPasswordValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty();

    RuleFor(x => x.NewPassword)
        .NotEmpty()
        .MinimumLength(6);
  }
}
