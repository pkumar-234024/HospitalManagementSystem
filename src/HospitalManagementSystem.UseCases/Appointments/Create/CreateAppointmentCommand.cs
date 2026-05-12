using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Create;

public record CreateAppointmentCommand(
  Guid HospitalId,
  string PatientName,
  string PatientEmail,
  string PatientPhoneNumber,
  string DoctorUserId,
  DateTimeOffset AppointmentDateTime,
  string Reason) : ICommand<Result<AppointmentDto>>;
