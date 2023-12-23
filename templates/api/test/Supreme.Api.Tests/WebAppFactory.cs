using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#if (database == "mysql")
using Testcontainers.MySql;
#endif
#if (database == "postgres")
using Testcontainers.PostgreSql;
#endif
#if (enable-outbox-pattern)
using Testcontainers.RabbitMq;
#endif
using Supreme.Infrastructure.Db;
using Xunit;

namespace Supreme.Api.Tests;

public sealed class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
#if (database == "mysql")
    private readonly MySqlContainer _dbContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithCleanUp(true)
        .WithDatabase("Belindax")
        .WithUsername("sqlsa")
        .WithPassword("SuperPass1")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
        .Build();
#endif
#if (database == "postgres")
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .WithDatabase("Supreme")
        .WithUsername("sqlsa")
        .WithPassword("SuperPass1")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();
#endif
#if (enable-outbox-pattern)
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("heidiks/rabbitmq-delayed-message-exchange")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .WithCleanUp(true)
        .Build();
#endif

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor =
                services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<SupremeContext>));

            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

#if (database == "mysql")
            services.AddDbContextPool<BelindaxContext>(opt =>
                {
                    var connectionString = $"{_dbContainer.GetConnectionString()};IgnoreCommandTransaction=true";
                    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
            );
#endif
#if (database == "postgres")
            services.AddDbContextPool<SupremeContext>(opt =>
                opt.UseNpgsql(_dbContainer.GetConnectionString()));
#endif            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SupremeContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
#if (enable-outbox-pattern)
        await _rabbitMqContainer.StopAsync();
#endif 
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
#if (enable-outbox-pattern)
        await _rabbitMqContainer.StopAsync();
#endif 
    }
}
