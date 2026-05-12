using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Appointments;
using HospitalManagementSystem.UseCases.Appointments.Approve;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HospitalManagementSystem.Web.Appointments;

public class ApproveAppointment(IMediator mediator, UserManager<ApplicationUser> userManager)
  : Endpoint<ApproveAppointmentRequest, Results<Ok<AppointmentDto>, NotFound, ForbidHttpResult, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;
  private readonly UserManager<ApplicationUser> _userManager = userManager;

  public override void Configure()
  {
    Post(ApproveAppointmentRequest.Route);
    Roles("Admin", "Doctor", "Receptionist");
    Tags("Appointments");
    Summary(s =>
    {
      s.Summary = "Approve an appointment";
      s.Description = "Approves a pending appointment. Doctors can only approve appointments assigned to them.";
      s.Responses[200] = "Appointment approved";
      s.Responses[403] = "You do not have permission to approve this appointment";
      s.Responses[404] = "Appointment not found";
    });
  }

  public override async Task<Results<Ok<AppointmentDto>, NotFound, ForbidHttpResult, ProblemHttpResult>> ExecuteAsync(
    ApproveAppointmentRequest request,
    CancellationToken cancellationToken)
  {
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    var currentUser = await _userManager.FindByIdAsync(currentUserId);
    var canReviewAcrossHospitals = currentUser is not null && currentUser.HospitalId is null && User.IsInRole("Admin");
    var isDoctor = User.IsInRole("Doctor");

    var result = await _mediator.Send(
      new ApproveAppointmentCommand(request.AppointmentId, currentUserId, currentUser?.HospitalId, canReviewAcrossHospitals, isDoctor),
      cancellationToken);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(result.Value),
      ResultStatus.NotFound => TypedResults.NotFound(),
      ResultStatus.Forbidden => TypedResults.Forbid(),
      _ => TypedResults.Problem(
        title: "Unable to approve appointment",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public sealed class ApproveAppointmentRequest
{
  public const string Route = "/appointments/{appointmentId:guid}/approve";

  [BindFrom("appointmentId")]
  public Guid AppointmentId { get; init; }
}
