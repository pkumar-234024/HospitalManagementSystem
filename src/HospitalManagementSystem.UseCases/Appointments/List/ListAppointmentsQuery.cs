using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.List;

public record ListAppointmentsQuery(
  int? Page = 1,
  int? PerPage = Constants.DEFAULT_PAGE_SIZE,
  Guid? HospitalId = null,
  bool CanViewAllHospitals = false,
  AppointmentStatus? Status = null)
  : IQuery<Result<UseCases.PagedResult<AppointmentDto>>>;
