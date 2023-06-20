using Supreme.Domain.Exceptions;
using Supreme.Domain.Outbox;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
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
        if (transaction is not MySqlTransaction mySqlTransaction)
        {
            throw new DevelopmentException($"{nameof(IDbTransaction)} is not a {nameof(MySqlTransaction)}");
        }
        
        _capPublisher.Transaction.Value = ActivatorUtilities.CreateInstance<MySqlCapTransaction>(_capPublisher.ServiceProvider);
        var capTransaction = _capPublisher.Transaction.Value.Begin(mySqlTransaction, false);
        var capOutboxTransaction = new OutboxTransaction(capTransaction);
        
        return capOutboxTransaction;    
    }
}
