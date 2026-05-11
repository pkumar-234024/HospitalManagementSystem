using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Approve;

public record ApproveAppointmentCommand(
  Guid AppointmentId,
  string ReviewedByUserId,
  bool CanReviewAnyAppointment) : ICommand<Result<AppointmentDto>>;
