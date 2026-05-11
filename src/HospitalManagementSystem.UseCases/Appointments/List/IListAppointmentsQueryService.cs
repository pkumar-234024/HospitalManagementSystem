using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.List;

public interface IListAppointmentsQueryService
{
  Task<UseCases.PagedResult<AppointmentDto>> ListAsync(
    int page,
    int perPage,
    HospitalManagementSystem.Core.AppointmentAggregate.AppointmentStatus? status = null);
}
