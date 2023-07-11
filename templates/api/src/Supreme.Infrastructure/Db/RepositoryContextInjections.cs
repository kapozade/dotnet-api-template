using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supreme.Domain.Db;

namespace Supreme.Infrastructure.Db;

public static class RepositoryContextInjections
{
    public static void AddRepositoryInjections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<SupremeContext>(
            (serviceProvider, dbContextOptions) =>
            {
#if DEBUG
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                dbContextOptions.UseLoggerFactory(loggerFactory);
#endif
#if (database == "mysql")
                   dbContextOptions
                    .UseMySql(configuration.GetConnectionString("DatabaseConnection"),
                        ServerVersion.AutoDetect(configuration.GetConnectionString("DatabaseConnection")));
#endif
#if (database == "postgres")
                   dbContextOptions
                    .UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));
#endif
            }
        );

        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}
