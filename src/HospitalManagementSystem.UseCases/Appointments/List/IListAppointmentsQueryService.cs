using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.List;

public interface IListAppointmentsQueryService
{
  Task<UseCases.PagedResult<AppointmentDto>> ListAsync(
    int page,
    int perPage,
    Guid? hospitalId = null,
    bool canViewAllHospitals = false,
    HospitalManagementSystem.Core.AppointmentAggregate.AppointmentStatus? status = null);
}
