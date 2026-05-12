using HospitalManagementSystem.Core.Model.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Web.Doctors;

public class ListDoctors(UserManager<ApplicationUser> userManager)
    : Endpoint<ListDoctorsRequest, Ok<List<DoctorRecord>>>
{
  private readonly UserManager<ApplicationUser> _userManager = userManager;

  public override void Configure()
  {
    Get("/doctors");

    AllowAnonymous();

    Tags("Doctors");

    Summary(s =>
    {
      s.Summary = "List available doctors";
      s.Description =
          "Returns all users assigned to the Doctor role so patients can book appointments.";

      s.Responses[200] = "Doctors returned";
    });
  }

  public override async Task<Ok<List<DoctorRecord>>>
      ExecuteAsync(ListDoctorsRequest request, CancellationToken cancellationToken)
  {
    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");

    if (request.HospitalId.HasValue)
    {
      doctors = doctors.Where(x => x.HospitalId == request.HospitalId.Value).ToList();
    }

    var response = doctors
        .OrderBy(x => x.FullName)
        .Select(x => new DoctorRecord(
            x.Id.ToString(),
            x.FullName,
            x.Email ?? string.Empty,
            x.PhoneNumber ?? string.Empty,
            x.HospitalId))
        .ToList();

    return TypedResults.Ok(response);
  }
}

public sealed class ListDoctorsRequest
{
  [BindFrom("hospitalId")]
  public Guid? HospitalId { get; init; }
}

public record DoctorRecord(
    string UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    Guid? HospitalId);
