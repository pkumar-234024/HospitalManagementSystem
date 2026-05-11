using Ardalis.ListStartupServices;
using HospitalManagementSystem.Core.Model.User;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

namespace HospitalManagementSystem.Web.Configurations;

public static class MiddlewareConfig
{
  public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware();
    }
    else
    {
      app.UseDefaultExceptionHandler();
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseFastEndpoints();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwaggerGen(options =>
      {
        options.Path = "/openapi/{documentName}.json";
      },
      settings =>
      {
        settings.Path = "/swagger";
        settings.DocumentPath = "/openapi/{documentName}.json";
      });

      app.MapScalarApiReference(options =>
      {
        options.WithTitle("Clean Architecture API");
        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
      });
    }

    var shouldMigrate = app.Environment.IsDevelopment() ||
                        app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

    if (shouldMigrate)
    {
      await MigrateDatabaseAsync(app);
      await SeedDatabaseAsync(app);
    }

    return app;
  }

  static async Task MigrateDatabaseAsync(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
      logger.LogInformation("Applying database migrations...");
      var context = services.GetRequiredService<AppDbContext>();

      if (context.Database.IsSqlite())
      {
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("SQLite database created successfully");
      }
      else
      {
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
      throw;
    }
  }

  static async Task SeedDatabaseAsync(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    try
    {
      logger.LogInformation("Seeding database...");
      var context = services.GetRequiredService<AppDbContext>();
      await SeedData.InitializeAsync(context);
      await IdentitySeeder.SeedAsync(userManager, roleManager);
      logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }
}
