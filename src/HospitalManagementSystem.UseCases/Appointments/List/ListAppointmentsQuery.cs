using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.List;

public record ListAppointmentsQuery(int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE, AppointmentStatus? Status = null)
  : IQuery<Result<UseCases.PagedResult<AppointmentDto>>>;
