using HospitalManagementSystem.Core.Interfaces;
using HospitalManagementSystem.Core.Services;
using HospitalManagementSystem.Infrastructure.Data;
using HospitalManagementSystem.Infrastructure.Data.Queries;
using HospitalManagementSystem.UseCases.Appointments.List;
using HospitalManagementSystem.UseCases.Contributors.List;

namespace HospitalManagementSystem.Infrastructure;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    bool isWindows = OperatingSystem.IsWindows();
    bool forceSqlServer = Environment.GetEnvironmentVariable("USE_SQL_SERVER") == "true";

    string? connectionString = config.GetConnectionString("cleanarchitecture")
                               ?? ((isWindows || forceSqlServer) ? config.GetConnectionString("DefaultConnection") : null)
                               ?? config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

      if (config.GetConnectionString("cleanarchitecture") != null ||
          ((isWindows || forceSqlServer) && config.GetConnectionString("DefaultConnection") != null))
      {
        options.UseSqlServer(connectionString);
      }
      else
      {
        options.UseSqlite(connectionString);
      }

      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
           .AddScoped<IListAppointmentsQueryService, ListAppointmentsQueryService>()
           .AddScoped<IDeleteContributorService, DeleteContributorService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
