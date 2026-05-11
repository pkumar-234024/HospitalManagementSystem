using FluentValidation;
using HospitalManagementSystem.UseCases.Appointments;
using HospitalManagementSystem.UseCases.Appointments.Create;
using HospitalManagementSystem.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Appointments;

public class Create(IMediator mediator)
  : Endpoint<CreateAppointmentRequest, Results<Created<AppointmentDto>, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post("/appointments");
    AllowAnonymous();
    Tags("Appointments");
    Summary(s =>
    {
      s.Summary = "Book a new appointment";
      s.Description = "Creates a pending appointment request without requiring a login.";
      s.ExampleRequest = new CreateAppointmentRequest
      {
        PatientName = "Jane Patient",
        PatientEmail = "jane@example.com",
        PatientPhoneNumber = "+1 555 123 4567",
        DoctorUserId = "doctor-user-id",
        AppointmentDateTime = DateTimeOffset.UtcNow.AddDays(1),
        Reason = "General consultation"
      };
      s.Responses[201] = "Appointment created";
    });
  }

  public override async Task<Results<Created<AppointmentDto>, ValidationProblem, ProblemHttpResult>> ExecuteAsync(
    CreateAppointmentRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateAppointmentCommand(
      request.PatientName,
      request.PatientEmail,
      request.PatientPhoneNumber,
      request.DoctorUserId,
      request.AppointmentDateTime,
      request.Reason), cancellationToken);

    return result.ToCreatedResult(
      dto => $"/appointments/{dto.Id}",
      dto => dto);
  }
}

public sealed class CreateAppointmentRequest
{
  public string PatientName { get; set; } = string.Empty;
  public string PatientEmail { get; set; } = string.Empty;
  public string PatientPhoneNumber { get; set; } = string.Empty;
  public string DoctorUserId { get; set; } = string.Empty;
  public DateTimeOffset AppointmentDateTime { get; set; }
  public string Reason { get; set; } = string.Empty;
}

public sealed class CreateAppointmentValidator : Validator<CreateAppointmentRequest>
{
  public CreateAppointmentValidator()
  {
    RuleFor(x => x.PatientName)
      .NotEmpty()
      .MaximumLength(100);

    RuleFor(x => x.PatientEmail)
      .NotEmpty()
      .EmailAddress()
      .MaximumLength(200);

    RuleFor(x => x.PatientPhoneNumber)
      .NotEmpty()
      .MaximumLength(25);

    RuleFor(x => x.DoctorUserId)
      .NotEmpty()
      .MaximumLength(450);

    RuleFor(x => x.AppointmentDateTime)
      .Must(value => value > DateTimeOffset.UtcNow)
      .WithMessage("Appointment date must be in the future.");

    RuleFor(x => x.Reason)
      .NotEmpty()
      .MaximumLength(1000);
  }
}
