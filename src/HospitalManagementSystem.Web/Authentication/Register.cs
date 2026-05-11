using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.UseCases.Authentication.Register;
using HospitalManagementSystem.UseCases.Dtos.Login;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HospitalManagementSystem.Web.Authentication;

public class Register(
    IMediator mediator,
    IJwtTokenService jwtTokenService)
    : Endpoint<
        RegisterRequest,
        Results<
            Created<LoginResponse>,
            ValidationProblem,
            BadRequest<string>>>
{
  private readonly IMediator _mediator = mediator;
  private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

  public override void Configure()
  {
    Post(RegisterRequest.Route);

    AllowAnonymous();

    Tags("Authentication");

    Summary(s =>
    {
      s.Summary = "User Registration";
      s.Description = "Registers a new user and returns authentication tokens.";

      s.ExampleRequest = new RegisterRequest
      {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Password = "Password@123",
        ConfirmPassword = "Password@123",
        PhoneNumber = "9876543210"
      };

      s.Responses[201] = "Registration successful";
      s.Responses[400] = "Validation failed or registration error";
    });

    Description(builder => builder
        .Accepts<RegisterRequest>("application/json")
        .Produces<LoginResponse>(201, "application/json")
        .ProducesProblem(400));
  }

  public override async Task<
      Results<
          Created<LoginResponse>,
          ValidationProblem,
          BadRequest<string>>>
      ExecuteAsync(
          RegisterRequest request,
          CancellationToken cancellationToken)
  {
    var command = new RegisterCommand
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      Password = request.Password,
      ConfirmPassword = request.ConfirmPassword,
      PhoneNumber = request.PhoneNumber!,
    };

    Result<LoginResponse> response =
        await _mediator.Send(command, cancellationToken);

    if (!response.IsSuccess)
    {
      var errorMessage =
          response.Errors.FirstOrDefault()
          ?? "Registration failed.";

      return TypedResults.BadRequest(errorMessage);
    }

    // Set secure authentication cookies
    _jwtTokenService.SetAuthenticationCookies(
        response.Value.AccessToken,
        response.Value.RefreshToken);

    return TypedResults.Created(
        RegisterRequest.Route,
        response.Value);
  }
}

public class RegisterRequest
{
  public const string Route = "/auth/register";

  [Required]
  [MaxLength(100)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [MaxLength(100)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  [Compare(nameof(Password))]
  public string ConfirmPassword { get; set; } = string.Empty;

  [Phone]
  public string? PhoneNumber { get; set; }
}

public class RegisterValidator : Validator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(x => x.FirstName)
        .NotEmpty()
        .MaximumLength(100);

    RuleFor(x => x.LastName)
        .NotEmpty()
        .MaximumLength(100);

    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

    RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(6);

    RuleFor(x => x.ConfirmPassword)
        .Equal(x => x.Password)
        .WithMessage("Passwords do not match.");

    RuleFor(x => x.PhoneNumber)
        .MaximumLength(20)
        .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
  }
}
