using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Decline;

public class DeclineAppointmentHandler(IRepository<Appointment> repository)
  : ICommandHandler<DeclineAppointmentCommand, Result<AppointmentDto>>
{
  public async ValueTask<Result<AppointmentDto>> Handle(DeclineAppointmentCommand command, CancellationToken cancellationToken)
  {
    var appointment = await repository.GetByIdAsync(command.AppointmentId, cancellationToken);
    if (appointment is null)
    {
      return Result.NotFound();
    }

    if (!command.CanReviewAnyAppointment && appointment.DoctorUserId != command.ReviewedByUserId)
    {
      return Result.Forbidden();
    }

    try
    {
      appointment.Decline(command.ReviewedByUserId, command.Reason);
      await repository.UpdateAsync(appointment, cancellationToken);
      return Result.Success(AppointmentDto.FromEntity(appointment));
    }
    catch (InvalidOperationException ex)
    {
      return Result.Error(ex.Message);
    }
  }
}
