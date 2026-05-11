using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Decline;

public record DeclineAppointmentCommand(
  Guid AppointmentId,
  string ReviewedByUserId,
  bool CanReviewAnyAppointment,
  string? Reason) : ICommand<Result<AppointmentDto>>;
