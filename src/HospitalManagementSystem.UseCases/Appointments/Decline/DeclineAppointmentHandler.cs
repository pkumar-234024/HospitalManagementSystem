using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.HospitalAggregate;
using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Decline;

public class DeclineAppointmentHandler(
  IRepository<Appointment> repository,
  IRepository<Hospital> hospitalRepository)
  : ICommandHandler<DeclineAppointmentCommand, Result<AppointmentDto>>
{
  public async ValueTask<Result<AppointmentDto>> Handle(DeclineAppointmentCommand command, CancellationToken cancellationToken)
  {
    var appointment = await repository.GetByIdAsync(command.AppointmentId, cancellationToken);
    if (appointment is null)
    {
      return Result.NotFound();
    }

    if (!command.CanReviewAcrossHospitals && !command.ReviewerHospitalId.HasValue)
    {
      return Result.Forbidden();
    }

    if (!command.CanReviewAcrossHospitals && command.ReviewerHospitalId.HasValue && appointment.HospitalId != command.ReviewerHospitalId.Value)
    {
      return Result.Forbidden();
    }

    if (command.IsDoctor && appointment.DoctorUserId != command.ReviewedByUserId)
    {
      return Result.Forbidden();
    }

    try
    {
      appointment.Decline(command.ReviewedByUserId, command.Reason);
      await repository.UpdateAsync(appointment, cancellationToken);

      var hospital = await hospitalRepository.GetByIdAsync(appointment.HospitalId, cancellationToken);
      return Result.Success(AppointmentDto.FromEntity(appointment, string.Empty, hospital?.Name ?? string.Empty));
    }
    catch (InvalidOperationException ex)
    {
      return Result.Error(ex.Message);
    }
  }
}
