using FluentValidation;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Appointments;
using HospitalManagementSystem.UseCases.Appointments.Decline;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HospitalManagementSystem.Web.Appointments;

public class DeclineAppointment(IMediator mediator, UserManager<ApplicationUser> userManager)
  : Endpoint<DeclineAppointmentRequest, Results<Ok<AppointmentDto>, NotFound, ForbidHttpResult, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;
  private readonly UserManager<ApplicationUser> _userManager = userManager;

  public override void Configure()
  {
    Post(DeclineAppointmentRequest.Route);
    Roles("Admin", "Doctor", "Receptionist");
    Tags("Appointments");
    Summary(s =>
    {
      s.Summary = "Decline an appointment";
      s.Description = "Declines a pending appointment with an optional note.";
      s.Responses[200] = "Appointment declined";
      s.Responses[403] = "You do not have permission to decline this appointment";
      s.Responses[404] = "Appointment not found";
    });
  }

  public override async Task<Results<Ok<AppointmentDto>, NotFound, ForbidHttpResult, ProblemHttpResult>> ExecuteAsync(
    DeclineAppointmentRequest request,
    CancellationToken cancellationToken)
  {
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    var currentUser = await _userManager.FindByIdAsync(currentUserId);
    var canReviewAcrossHospitals = currentUser is not null && currentUser.HospitalId is null && User.IsInRole("Admin");
    var isDoctor = User.IsInRole("Doctor");

    var result = await _mediator.Send(
      new DeclineAppointmentCommand(request.AppointmentId, currentUserId, currentUser?.HospitalId, canReviewAcrossHospitals, isDoctor, request.Reason),
      cancellationToken);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(result.Value),
      ResultStatus.NotFound => TypedResults.NotFound(),
      ResultStatus.Forbidden => TypedResults.Forbid(),
      _ => TypedResults.Problem(
        title: "Unable to decline appointment",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public sealed class DeclineAppointmentRequest
{
  public const string Route = "/appointments/{appointmentId:guid}/decline";

  [BindFrom("appointmentId")]
  public Guid AppointmentId { get; init; }

  public string? Reason { get; set; }
}

public sealed class DeclineAppointmentValidator : Validator<DeclineAppointmentRequest>
{
  public DeclineAppointmentValidator()
  {
    RuleFor(x => x.Reason)
      .MaximumLength(1000)
      .When(x => !string.IsNullOrWhiteSpace(x.Reason));
  }
}
