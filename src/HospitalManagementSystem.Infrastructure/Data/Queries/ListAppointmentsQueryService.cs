using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.UseCases.Appointments;
using HospitalManagementSystem.UseCases.Appointments.List;

namespace HospitalManagementSystem.Infrastructure.Data.Queries;

public class ListAppointmentsQueryService : IListAppointmentsQueryService
{
  private readonly AppDbContext _db;

  public ListAppointmentsQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<UseCases.PagedResult<AppointmentDto>> ListAsync(
    int page,
    int perPage,
    Guid? hospitalId = null,
    bool canViewAllHospitals = false,
    AppointmentStatus? status = null)
  {
    var baseQuery =
      from appointment in _db.Appointments.AsNoTracking()
      join doctor in _db.Users.AsNoTracking()
        on appointment.DoctorUserId equals doctor.Id into doctors
      from doctor in doctors.DefaultIfEmpty()
      join hospital in _db.Hospitals.AsNoTracking()
        on appointment.HospitalId equals hospital.Id into hospitals
      from hospital in hospitals.DefaultIfEmpty()
      select new
      {
        Appointment = appointment,
        DoctorName = doctor == null ? string.Empty : doctor.FullName,
        HospitalName = hospital == null ? string.Empty : hospital.Name
      };

    if (!canViewAllHospitals && hospitalId.HasValue)
    {
      baseQuery = baseQuery.Where(x => x.Appointment.HospitalId == hospitalId.Value);
    }

    if (status.HasValue)
    {
      baseQuery = baseQuery.Where(x => x.Appointment.Status == status.Value);
    }

    var totalCount = await baseQuery.CountAsync();
    var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)perPage);

    var items = (await baseQuery
      .OrderByDescending(x => x.Appointment.AppointmentDateTime)
      .Skip((page - 1) * perPage)
      .Take(perPage)
      .Select(x => new
      {
        x.Appointment.Id,
        x.Appointment.HospitalId,
        x.Appointment.PatientName,
        x.Appointment.PatientEmail,
        x.Appointment.PatientPhoneNumber,
        x.Appointment.DoctorUserId,
        x.DoctorName,
        x.HospitalName,
        x.Appointment.AppointmentDateTime,
        x.Appointment.Reason,
        x.Appointment.Status,
        x.Appointment.ReviewedByUserId,
        x.Appointment.ReviewedAt,
        x.Appointment.DecisionNote,
        x.Appointment.CreatedAt,
        x.Appointment.UpdatedAt
      })
      .ToListAsync())
      .Select(x => new AppointmentDto(
        x.Id,
        x.HospitalId,
        x.HospitalName,
        x.PatientName,
        x.PatientEmail,
        x.PatientPhoneNumber,
        x.DoctorUserId,
        x.DoctorName,
        x.AppointmentDateTime,
        x.Reason,
        x.Status.ToString(),
        x.ReviewedByUserId,
        x.ReviewedAt,
        x.DecisionNote,
        x.CreatedAt,
        x.UpdatedAt))
      .ToList();

    return new UseCases.PagedResult<AppointmentDto>(items, page, perPage, totalCount, totalPages);
  }
}
