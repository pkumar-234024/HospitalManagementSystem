using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.UseCases.Authentication.Dtos;
using HospitalManagementSystem.UseCases.Authentication.Login;
using HospitalManagementSystem.UseCases.Dtos.Login;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class Login(IMediator mediator, IJwtTokenService jwtTokenService)
    : Endpoint<
        LoginRequest,
        Results<
            Ok<LoginResponse>,
            ValidationProblem,
            BadRequest<string>>>
{
  private readonly IMediator _mediator = mediator;
  private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

  public override void Configure()
  {
    Post(LoginRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "User Login";
      s.Description = "Authenticates a user and returns access and refresh tokens.";

      s.ExampleRequest = new LoginRequest
      {
        Email = "admin@hospital.com",
        Password = "Admin@123"
      };

      s.Responses[200] = "Login successful";
      s.Responses[400] = "Invalid credentials or validation errors";
    });

    Description(builder => builder
        .Accepts<LoginRequest>("application/json")
        .Produces<LoginResponse>(200, "application/json")
        .ProducesProblem(400));
  }

  public override async Task<
      Results<
          Ok<LoginResponse>,
          ValidationProblem,
          BadRequest<string>>>
      ExecuteAsync(
          LoginRequest request,
          CancellationToken cancellationToken)
  {
    var command = new LoginCommand
    {
      Email = request.Email,
      Password = request.Password
    };

    Result<LoginResponse> response =
        await _mediator.Send(command, cancellationToken);

    if (!response.IsSuccess)
    {
      var errorMessage = response.Errors.FirstOrDefault()
                         ?? "Invalid email or password.";

      return TypedResults.BadRequest(errorMessage);
    }

    // Set secure authentication cookies
    _jwtTokenService.SetAuthenticationCookies(
        response.Value.AccessToken,
        response.Value.RefreshToken);

    return TypedResults.Ok(response.Value);
  }
}

public class LoginRequest
{
  public const string Route = "/auth/login";

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}

public class LoginValidator : Validator<LoginRequest>
{
  public LoginValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

    RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(6);
  }
}
