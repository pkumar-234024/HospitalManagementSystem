using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.HospitalAggregate;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.UseCases.Appointments;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.UseCases.Appointments.Create;

public class CreateAppointmentHandler(
  IRepository<Hospital> hospitalRepository,
  IRepository<Appointment> repository,
  UserManager<ApplicationUser> userManager)
  : ICommandHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
  public async ValueTask<Result<AppointmentDto>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
  {
    var hospital = await hospitalRepository.GetByIdAsync(command.HospitalId, cancellationToken);
    if (hospital is null || !hospital.IsActive)
    {
      return Result.Error("Selected hospital is not available.");
    }

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

    if (doctor.HospitalId != command.HospitalId)
    {
      return Result.Error("Selected doctor does not belong to the chosen hospital.");
    }

    var appointment = new Appointment(
      command.HospitalId,
      command.PatientName.Trim(),
      command.PatientEmail.Trim().ToLowerInvariant(),
      command.PatientPhoneNumber.Trim(),
      command.DoctorUserId.Trim(),
      command.AppointmentDateTime,
      command.Reason.Trim());

    var created = await repository.AddAsync(appointment, cancellationToken);

    return Result.Success(AppointmentDto.FromEntity(created, string.Empty, hospital.Name));
  }
}
