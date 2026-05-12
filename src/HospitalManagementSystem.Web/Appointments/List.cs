using FluentValidation;
using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Appointments;
using HospitalManagementSystem.UseCases.Appointments.List;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HospitalManagementSystem.Web.Appointments;

public class ListAppointments(IMediator mediator, UserManager<ApplicationUser> userManager)
  : Endpoint<ListAppointmentsRequest, Results<Ok<UseCases.PagedResult<AppointmentDto>>, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;
  private readonly UserManager<ApplicationUser> _userManager = userManager;

  public override void Configure()
  {
    Get("/appointments");
    Roles("Admin", "Doctor", "Receptionist");
    Tags("Appointments");
    Summary(s =>
    {
      s.Summary = "List appointments";
      s.Description = "Returns a paginated list of appointments for staff review.";
      s.Responses[200] = "Appointments returned";
    });
  }

  public override async Task<Results<Ok<UseCases.PagedResult<AppointmentDto>>, ValidationProblem, ProblemHttpResult>> ExecuteAsync(
    ListAppointmentsRequest request,
    CancellationToken cancellationToken)
  {
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    var currentUser = await _userManager.FindByIdAsync(currentUserId);
    var canViewAllHospitals = currentUser is not null && currentUser.HospitalId is null && User.IsInRole("Admin");

    var result = await _mediator.Send(new ListAppointmentsQuery(
      request.Page,
      request.PerPage,
      currentUser?.HospitalId,
      canViewAllHospitals,
      request.Status), cancellationToken);

    if (!result.IsSuccess)
    {
      return TypedResults.Problem(
        title: "Unable to list appointments",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest);
    }

    return TypedResults.Ok(result.Value);
  }
}

public sealed class ListAppointmentsRequest
{
  [BindFrom("page")]
  public int Page { get; init; } = 1;

  [BindFrom("per_page")]
  public int PerPage { get; init; } = 10;

  [BindFrom("status")]
  public AppointmentStatus? Status { get; init; }
}

public sealed class ListAppointmentsValidator : Validator<ListAppointmentsRequest>
{
  public ListAppointmentsValidator()
  {
    RuleFor(x => x.Page)
      .GreaterThanOrEqualTo(1);

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, 100);
  }
}
