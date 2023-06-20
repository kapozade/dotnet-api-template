namespace Supreme.Domain.Outbox;

public interface IOutboxTransaction : IDisposable
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
