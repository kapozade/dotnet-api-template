using Supreme.Domain.Outbox;
using DotNetCore.CAP;

namespace Supreme.Infrastructure.Outbox;

public sealed class OutboxTransaction : IOutboxTransaction
{
    private ICapTransaction _capTransaction;

    public OutboxTransaction(
        ICapTransaction capTransaction
        )
    {
        _capTransaction = capTransaction;
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken)
    {
        await _capTransaction.CommitAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (!disposing || _capTransaction == null) return;

        _capTransaction.Dispose();
        _capTransaction = null!;
    }
}
