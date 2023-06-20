using Supreme.Domain;
using Supreme.Domain.IntegrationEvents;
using DotNetCore.CAP;

namespace Supreme.Api.Consumers;

public class FooIntegrationEventConsumer : ICapSubscribe
{
    public FooIntegrationEventConsumer()
    {
    }

    [CapSubscribe(FooIntegrationEvent._eventName,
        Group = nameof(FooIntegrationEvent) + nameof(TryHandleAsync))]
    public Task TryHandleAsync(FooIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        // TODO: Implement your event.
        return Task.FromResult(Task.CompletedTask);
    }
}