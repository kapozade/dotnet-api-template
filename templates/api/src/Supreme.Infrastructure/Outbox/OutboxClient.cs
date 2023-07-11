using Supreme.Domain.Exceptions;
using Supreme.Domain.Outbox;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
#if (database == "mysql")
using MySqlConnector;
#endif
#if (database == "postgres")
using Npgsql;
#endif
using System.Data;

namespace Supreme.Infrastructure.Outbox;

public sealed class OutboxClient : IOutboxClient
{
    private readonly ICapPublisher _capPublisher;

    public OutboxClient(
        ICapPublisher capPublisher
        )
    {
        _capPublisher = capPublisher;
    }

    public async Task AddAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IOutboxMessage
    {
        await _capPublisher.PublishAsync(@event.EventName, @event, cancellationToken: cancellationToken);
    }

    public async Task AddDelayedAsync<TEvent>(TEvent @event, int delayInSeconds, CancellationToken cancellationToken) where TEvent : IOutboxDelayedMessage
    {
        await _capPublisher.PublishDelayAsync(TimeSpan.FromSeconds(delayInSeconds), @event.EventName, @event, @event.CallbackName, cancellationToken);
    }

    public IOutboxTransaction UseTransaction(IDbTransaction transaction)
    {
#if (database == "mysql")
        if (transaction is not MySqlTransaction mySqlTransaction)
        {
            throw new DevelopmentException($"{nameof(IDbTransaction)} is not a {nameof(MySqlTransaction)}");
        }
        
        _capPublisher.Transaction.Value = ActivatorUtilities.CreateInstance<MySqlCapTransaction>(_capPublisher.ServiceProvider);
        var capTransaction = _capPublisher.Transaction.Value.Begin(mySqlTransaction, false);
        var capOutboxTransaction = new OutboxTransaction(capTransaction);
#endif
#if (database == "postgres")
        if (transaction is not NpgsqlTransaction postgresTransaction)
        {
            throw new DevelopmentException($"{nameof(IDbTransaction)} is not a {nameof(NpgsqlTransaction)}");
        }
        
        _capPublisher.Transaction.Value = ActivatorUtilities.CreateInstance<PostgreSqlCapTransaction>(_capPublisher.ServiceProvider);
        var capTransaction = _capPublisher.Transaction.Value.Begin(postgresTransaction, false);
        var capOutboxTransaction = new OutboxTransaction(capTransaction);
#endif

        return capOutboxTransaction;    
    }
}
