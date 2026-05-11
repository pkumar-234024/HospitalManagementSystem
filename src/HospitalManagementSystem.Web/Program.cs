using System.Text;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Core.Options;
using HospitalManagementSystem.Infrastructure.Data;
using HospitalManagementSystem.ServiceDefaults;
using HospitalManagementSystem.Web.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults()    // This sets up OpenTelemetry logging
       .AddLoggerConfigs();     // This adds Serilog for console formatting
using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();
startupLogger.LogInformation("Starting web host");
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.Section));
builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
      options.Password.RequireDigit = true;
      options.Password.RequireNonAlphanumeric = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireLowercase = true;
      options.Password.RequiredLength = 8;
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
      options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// Configure JWT
var jwtSection = builder.Configuration.GetSection(JwtOptions.Section);
builder.Services.Configure<JwtOptions>(jwtSection);

var jwtOptions = jwtSection.Get<JwtOptions>();
var key = Encoding.ASCII.GetBytes(jwtOptions?.SecretKey ?? string.Empty);
builder.Services
    .AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = jwtOptions?.ValidateIssuerSigningKey ?? false,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = jwtOptions?.ValidateIssuer ?? false,
        ValidIssuer = jwtOptions?.Issuer,
        ValidateAudience = jwtOptions?.ValidateAudience ?? false,
        ValidAudience = jwtOptions?.Audience,
        ValidateLifetime = jwtOptions?.ValidateLifetime ?? false,
        ClockSkew = TimeSpan.Zero
      };
      // Support token from cookies
      options.Events = new JwtBearerEvents
      {
        OnMessageReceived = context =>
        {
          if (context.Request.Cookies.TryGetValue("accessToken", out var token))
          {
            context.Token = token;
          }
          return System.Threading.Tasks.Task.CompletedTask;
        }
      };
    });
builder.Services.AddAuthorization();
// Configure Gmail OAuth
builder.Services.Configure<GmailOptions>(builder.Configuration.GetSection(GmailOptions.Section));
// Configure Email
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.Section));
builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.DocumentSettings = s =>
                  {
                    s.Title = "Clean Architecture API";
                    s.Version = "v1";
                    s.Description = "HTTP endpoints for the Clean Architecture sample application.";
                  };
                  o.ShortSchemaNames = true;
                });

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

    if (origins.Length > 0)
    {
      policy.WithOrigins(origins);
    }
    else
    {
      policy.WithOrigins(
        "http://localhost:8080",
        "https://localhost:8080",
        "http://127.0.0.1:8080",
        "https://127.0.0.1:8080",
        "http://localhost:5173",
        "https://localhost:5173",
        "http://127.0.0.1:5173",
        "https://127.0.0.1:5173");
    }

    policy
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
  });
});

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();

app.MapDefaultEndpoints(); // Aspire health checks and metrics
app.UseCors("AllowFrontend");
app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
