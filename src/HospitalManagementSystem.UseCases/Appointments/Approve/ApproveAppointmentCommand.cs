using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Approve;

public record ApproveAppointmentCommand(
  Guid AppointmentId,
  string ReviewedByUserId,
  Guid? ReviewerHospitalId,
  bool CanReviewAcrossHospitals,
  bool IsDoctor) : ICommand<Result<AppointmentDto>>;
