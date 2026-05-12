using System.Security.Claims;
using HospitalManagementSystem.Infrastructure.Data;
using HospitalManagementSystem.UseCases.Authentication.Dtos;
using HospitalManagementSystem.UseCases.Authentication.GetAuthStatus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Authentication;

public class GetAuthStatus(IMediator mediator, AppDbContext db)
    : EndpointWithoutRequest<
        Results<
            Ok<AuthStatusResponse>,
            UnauthorizedHttpResult>>
{
  private readonly IMediator _mediator = mediator;
  private readonly AppDbContext _db = db;

  public override void Configure()
  {
    Get("/auth/status");

    Roles();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Get Authentication Status";
      s.Description = "Returns current authenticated user status.";

      s.Responses[200] = "Authentication status retrieved successfully";
      s.Responses[401] = "Unauthorized";
    });

    Description(builder => builder
        .Produces<AuthStatusResponse>(200, "application/json")
        .Produces(401));
  }

  public override async Task<
      Results<
          Ok<AuthStatusResponse>,
          UnauthorizedHttpResult>>
      ExecuteAsync(CancellationToken cancellationToken)
  {
    var userId =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userId))
    {
      return TypedResults.Unauthorized();
    }

    var query = new GetAuthStatusQuery
    {
      UserId = userId
    };

    var response =
        await _mediator.Send(query, cancellationToken);

    if (response.HospitalId.HasValue)
    {
      response.HospitalName = await _db.Hospitals.AsNoTracking()
        .Where(x => x.Id == response.HospitalId.Value)
        .Select(x => x.Name)
        .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
    }

    return TypedResults.Ok(response);
  }
}
