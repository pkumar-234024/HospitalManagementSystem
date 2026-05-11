using Ardalis.SharedKernel;

namespace HospitalManagementSystem.Core.AppointmentAggregate;

public class Appointment : IAggregateRoot
{
  private Appointment() { }

  public Appointment(
    string patientName,
    string patientEmail,
    string patientPhoneNumber,
    string doctorUserId,
    DateTimeOffset appointmentDateTime,
    string reason)
  {
    Id = Guid.NewGuid();
    PatientName = patientName;
    PatientEmail = patientEmail;
    PatientPhoneNumber = patientPhoneNumber;
    DoctorUserId = doctorUserId;
    AppointmentDateTime = appointmentDateTime;
    Reason = reason;
    Status = AppointmentStatus.Pending;
    CreatedAt = DateTimeOffset.UtcNow;
  }

  public Guid Id { get; private set; }
  public string PatientName { get; private set; } = string.Empty;
  public string PatientEmail { get; private set; } = string.Empty;
  public string PatientPhoneNumber { get; private set; } = string.Empty;
  public string DoctorUserId { get; private set; } = string.Empty;
  public DateTimeOffset AppointmentDateTime { get; private set; }
  public string Reason { get; private set; } = string.Empty;
  public AppointmentStatus Status { get; private set; }
  public string? ReviewedByUserId { get; private set; }
  public DateTimeOffset? ReviewedAt { get; private set; }
  public string? DecisionNote { get; private set; }
  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset? UpdatedAt { get; private set; }

  public void Approve(string reviewedByUserId)
  {
    EnsurePending();

    Status = AppointmentStatus.Approved;
    ReviewedByUserId = reviewedByUserId;
    ReviewedAt = DateTimeOffset.UtcNow;
    DecisionNote = null;
    UpdatedAt = DateTimeOffset.UtcNow;
  }

  public void Decline(string reviewedByUserId, string? note)
  {
    EnsurePending();

    Status = AppointmentStatus.Declined;
    ReviewedByUserId = reviewedByUserId;
    ReviewedAt = DateTimeOffset.UtcNow;
    DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    UpdatedAt = DateTimeOffset.UtcNow;
  }

  private void EnsurePending()
  {
    if (Status != AppointmentStatus.Pending)
    {
      throw new InvalidOperationException("Only pending appointments can be reviewed.");
    }
  }
}
