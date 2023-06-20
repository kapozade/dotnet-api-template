namespace Supreme.Domain.Outbox;

public interface IOutboxMessage
{
    public string EventName { get; }
}
