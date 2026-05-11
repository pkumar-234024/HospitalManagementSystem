using HospitalManagementSystem.Core.AppointmentAggregate;

namespace HospitalManagementSystem.UseCases.Appointments;

public record AppointmentDto(
  Guid Id,
  string PatientName,
  string PatientEmail,
  string PatientPhoneNumber,
  string DoctorUserId,
  string DoctorName,
  DateTimeOffset AppointmentDateTime,
  string Reason,
  string Status,
  string? ReviewedByUserId,
  DateTimeOffset? ReviewedAt,
  string? DecisionNote,
  DateTimeOffset CreatedAt,
  DateTimeOffset? UpdatedAt)
{
  public static AppointmentDto FromEntity(Appointment appointment, string doctorName = "")
    => new(
      appointment.Id,
      appointment.PatientName,
      appointment.PatientEmail,
      appointment.PatientPhoneNumber,
      appointment.DoctorUserId,
      doctorName,
      appointment.AppointmentDateTime,
      appointment.Reason,
      appointment.Status.ToString(),
      appointment.ReviewedByUserId,
      appointment.ReviewedAt,
      appointment.DecisionNote,
      appointment.CreatedAt,
      appointment.UpdatedAt);
}
