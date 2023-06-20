using Supreme.Domain.Outbox;

namespace Supreme.Domain.IntegrationEvents;

public record FooIntegrationEvent(long Id, int ExecutedBy, DateTime ExecutedOn) : IOutboxMessage
{
    public const string _eventName = "Supreme.FooIntegrationEvent";
    
    public string EventName
    {
        get => _eventName;
    }
}