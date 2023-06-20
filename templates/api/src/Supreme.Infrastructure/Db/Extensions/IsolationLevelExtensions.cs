using Supreme.Domain;
using Supreme.Domain.Enums;
using System.Data;

namespace Supreme.Infrastructure.Db.Extensions;

public static class IsolationLevelExtension
{
    public static IsolationLevel? ToIsolationLevel(this TransactionLevels? transactionLevel)
    {
        return transactionLevel switch
               {
                   null => null,
                   TransactionLevels.ReadUncommitted => IsolationLevel.ReadUncommitted,
                   TransactionLevels.ReadCommitted => IsolationLevel.ReadCommitted,
                   _ => throw new ArgumentOutOfRangeException(nameof(transactionLevel), transactionLevel, null)
               };
    }
}
