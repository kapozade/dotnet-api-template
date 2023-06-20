using Supreme.Domain;
using Supreme.Domain.IntegrationEvents;
using DotNetCore.CAP;

namespace Supreme.Api.Consumers;

public class FooDelayedIntegrationEventConsumer : ICapSubscribe
{
    public FooDelayedIntegrationEventConsumer()
    {
    }

    [CapSubscribe(FooDelayedIntegrationEvent._eventName,
        Group = nameof(FooDelayedIntegrationEvent) + nameof(TryHandleAsync))]
    public Task TryHandleAsync(FooDelayedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        // TODO: Implement your event.
        return Task.FromResult(Task.CompletedTask);
    }
}