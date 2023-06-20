namespace Supreme.Domain.Outbox;

public interface IOutboxDelayedMessage : IOutboxMessage
{
    public string? CallbackName { get; }
}
