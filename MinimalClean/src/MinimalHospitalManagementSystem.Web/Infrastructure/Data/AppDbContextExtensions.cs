using Microsoft.EntityFrameworkCore;

namespace MinimalHospitalManagementSystem.Web.Infrastructure.Data;

public static class AppDbContextExtensions
{
  public static void AddApplicationDbContext(this IServiceCollection services, string connectionString) =>
    services.AddDbContext<AppDbContext>(options =>
         options.UseSqlServer(connectionString));

}
