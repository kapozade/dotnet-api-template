using Supreme.Domain.Outbox;

namespace Supreme.Domain.IntegrationEvents;

public record FooDelayedIntegrationEvent(long Id, int ExecutedBy, DateTime ExecutedOn) : IOutboxDelayedMessage
{
    public const string _eventName = "Supreme.FooDelayedIntegrationEvent";
    
    public string EventName
    {
        get => _eventName;
    }

    public string? CallbackName { get => null; }
}