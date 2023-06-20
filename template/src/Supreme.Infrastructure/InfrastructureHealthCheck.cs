using Microsoft.Extensions.Diagnostics.HealthChecks;
using Supreme.Domain.Db;
#if (enable-outbox-pattern)
using RabbitMQ.Client;
#endif
using StackExchange.Redis;

namespace Supreme.Infrastructure;

internal sealed class InfrastructureHealthCheck : IHealthCheck
{
    private readonly IDatabase _redisDatabase;
#if (enable-outbox-pattern)
    private readonly IConnectionFactory _connectionFactory;
#endif
    private readonly IUnitOfWork _unitOfWork;

    public InfrastructureHealthCheck(
        IDatabase redisDatabase,
#if (enable-outbox-pattern)
        IConnectionFactory connectionFactory,
#endif
        IUnitOfWork unitOfWork
    )
    {
        _redisDatabase = redisDatabase;
#if (enable-outbox-pattern)
        _connectionFactory = connectionFactory;
#endif
        _unitOfWork = unitOfWork;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await _unitOfWork.CheckHealthAsync();
#if (enable-outbox-pattern)
        using var connection = _connectionFactory.CreateConnection();
        using var model = connection.CreateModel();
#endif        
        return !_redisDatabase.IsConnected("test") 
            ? new HealthCheckResult(HealthStatus.Unhealthy, "Redis not connected") 
            : new HealthCheckResult(HealthStatus.Healthy);
    }
}
