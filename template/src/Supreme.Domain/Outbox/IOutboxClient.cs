using System.Data;

namespace Supreme.Domain.Outbox;

public interface IOutboxClient
{
    Task AddAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IOutboxMessage;
    Task AddDelayedAsync<TEvent>(TEvent @event, int delayInSeconds, CancellationToken cancellationToken) where TEvent : IOutboxDelayedMessage;
    IOutboxTransaction UseTransaction(IDbTransaction transaction);
}
