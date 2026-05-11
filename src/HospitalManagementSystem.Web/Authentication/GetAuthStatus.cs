using System.Security.Claims;
using HospitalManagementSystem.UseCases.Authentication.Dtos;
using HospitalManagementSystem.UseCases.Authentication.GetAuthStatus;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class GetAuthStatus(IMediator mediator)
    : EndpointWithoutRequest<
        Results<
            Ok<AuthStatusResponse>,
            UnauthorizedHttpResult>>
{
  private readonly IMediator _mediator = mediator;

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

    return TypedResults.Ok(response);
  }
}
