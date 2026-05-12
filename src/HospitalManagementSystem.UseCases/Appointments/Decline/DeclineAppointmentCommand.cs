using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Decline;

public record DeclineAppointmentCommand(
  Guid AppointmentId,
  string ReviewedByUserId,
  Guid? ReviewerHospitalId,
  bool CanReviewAcrossHospitals,
  bool IsDoctor,
  string? Reason) : ICommand<Result<AppointmentDto>>;
