using HospitalManagementSystem.Infrastructure.Data;
using HospitalManagementSystem.UseCases.Dtos.Hospital;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Hospitals;

public class ListHospitals(AppDbContext db)
  : EndpointWithoutRequest<Ok<List<HospitalDto>>>
{
  private readonly AppDbContext _db = db;

  public override void Configure()
  {
    Get("/hospitals");
    AllowAnonymous();
    Tags("Hospitals");
    Summary(s =>
    {
      s.Summary = "List hospitals";
      s.Description = "Returns the available hospitals that patients can book into.";
      s.Responses[200] = "Hospitals returned";
    });
  }

  public override async Task<Ok<List<HospitalDto>>> ExecuteAsync(CancellationToken cancellationToken)
  {
    var hospitals = await _db.Hospitals.AsNoTracking()
      .Where(x => x.IsActive)
      .OrderBy(x => x.Name)
      .Select(x => new HospitalDto
      {
        Id = x.Id,
        Name = x.Name,
        Code = x.Code
      })
      .ToListAsync(cancellationToken);

    return TypedResults.Ok(hospitals);
  }
}
