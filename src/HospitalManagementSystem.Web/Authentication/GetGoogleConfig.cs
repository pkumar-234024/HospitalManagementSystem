using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class GetGoogleConfig(IConfiguration configuration)
    : EndpointWithoutRequest<
        Ok<GoogleConfigResponse>>
{
  private readonly IConfiguration _configuration = configuration;

  public override void Configure()
  {
    Get("/auth/google/config");

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "Get Google OAuth Configuration";
      s.Description = "Returns Google OAuth configuration for frontend authentication.";

      s.Responses[200] = "Google OAuth configuration retrieved successfully";
    });

    Description(builder => builder
        .Produces<GoogleConfigResponse>(200, "application/json"));
  }

  public override async Task<Ok<GoogleConfigResponse>>
      ExecuteAsync(CancellationToken cancellationToken)
  {
    var response = new GoogleConfigResponse
    {
      ClientId =
            _configuration["Gmail:ClientId"] ?? string.Empty,

      RedirectUri =
            _configuration["Gmail:RedirectUri"] ?? string.Empty,

      Scopes =
            _configuration
                .GetSection("Gmail:Scopes")
                .Get<string[]>()
            ?? Array.Empty<string>()
    };

    await Task.CompletedTask;

    return TypedResults.Ok(response);
  }
}

public class GoogleConfigResponse
{
  public string ClientId { get; set; } = string.Empty;

  public string RedirectUri { get; set; } = string.Empty;

  public string[] Scopes { get; set; } = Array.Empty<string>();
}
