using Supreme.Domain.Entities.Foo;
using Supreme.Domain.Enums;
using System.Data;

namespace Supreme.Domain.Db;

public interface IUnitOfWork : IDisposable
{
    IFooRepository FooRepository { get; }
    IDbTransaction BeginTransaction(TransactionLevels? transactionLevels = null);
    Task CommitChangesAsync(CancellationToken cancellationToken);
    Task CheckHealthAsync();
}
