using HospitalManagementSystem.UseCases.Appointments;

namespace HospitalManagementSystem.UseCases.Appointments.List;

public class ListAppointmentsHandler(IListAppointmentsQueryService queryService)
  : IQueryHandler<ListAppointmentsQuery, Result<UseCases.PagedResult<AppointmentDto>>>
{
  public async ValueTask<Result<UseCases.PagedResult<AppointmentDto>>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
  {
    var result = await queryService.ListAsync(
      request.Page ?? 1,
      request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
      request.Status);

    return Result.Success(result);
  }
}
