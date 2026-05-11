using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Appointments;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Appointments.Create;

public class CreateAppointmentHandler(
  IRepository<Appointment> repository,
  UserManager<ApplicationUser> userManager)
  : ICommandHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
  public async ValueTask<Result<AppointmentDto>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
  {
    var doctor = await userManager.FindByIdAsync(command.DoctorUserId);
    if (doctor is null)
    {
      return Result.Error("Selected doctor does not exist.");
    }

    var doctorRoles = await userManager.GetRolesAsync(doctor);
    if (!doctorRoles.Contains("Doctor"))
    {
      return Result.Error("Selected user is not a doctor.");
    }

    var appointment = new Appointment(
      command.PatientName.Trim(),
      command.PatientEmail.Trim().ToLowerInvariant(),
      command.PatientPhoneNumber.Trim(),
      command.DoctorUserId.Trim(),
      command.AppointmentDateTime,
      command.Reason.Trim());

    var created = await repository.AddAsync(appointment, cancellationToken);

    return Result.Success(AppointmentDto.FromEntity(created));
  }
}
