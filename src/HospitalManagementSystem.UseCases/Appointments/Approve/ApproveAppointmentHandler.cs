using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.Approve;

public class ApproveAppointmentHandler(IRepository<Appointment> repository)
  : ICommandHandler<ApproveAppointmentCommand, Result<AppointmentDto>>
{
  public async ValueTask<Result<AppointmentDto>> Handle(ApproveAppointmentCommand command, CancellationToken cancellationToken)
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
      appointment.Approve(command.ReviewedByUserId);
      await repository.UpdateAsync(appointment, cancellationToken);
      return Result.Success(AppointmentDto.FromEntity(appointment));
    }
    catch (InvalidOperationException ex)
    {
      return Result.Error(ex.Message);
    }
  }
}
