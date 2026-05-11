using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class Logout
    : EndpointWithoutRequest<
        Results<
            Ok<MessageResponse>,
            UnauthorizedHttpResult>>
{
  private const string AccessTokenCookie = "accessToken";
  private const string RefreshTokenCookie = "refreshToken";

  public override void Configure()
  {
    Post("/auth/logout");

    Roles();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Logout";
      s.Description = "Logs out the current user and clears authentication cookies.";

      s.Responses[200] = "Logged out successfully";
      s.Responses[401] = "Unauthorized";
    });

    Description(builder => builder
        .Produces<MessageResponse>(200, "application/json")
        .Produces(401));
  }

  public override async Task<
      Results<
          Ok<MessageResponse>,
          UnauthorizedHttpResult>>
      ExecuteAsync(CancellationToken cancellationToken)
  {
    var userId =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userId))
    {
      return TypedResults.Unauthorized();
    }

    // Clear authentication cookies
    HttpContext.Response.Cookies.Delete(AccessTokenCookie);
    HttpContext.Response.Cookies.Delete(RefreshTokenCookie);

    await Task.CompletedTask;

    return TypedResults.Ok(
        new MessageResponse
        {
          Message = "Logged out successfully"
        });
  }
}
