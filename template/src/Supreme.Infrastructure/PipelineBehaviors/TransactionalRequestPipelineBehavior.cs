using MediatR;
using Supreme.Domain;
using Supreme.Domain.Db;
using Supreme.Domain.Outbox;

namespace Supreme.Infrastructure.PipelineBehaviors;

public class TransactionalRequestPipelineBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> 
    where TReq : ITransactionalRequest<TRes> where TRes : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxClient _outboxClient;

    public TransactionalRequestPipelineBehavior(
        IUnitOfWork unitOfWork,
        IOutboxClient outboxClient
        )
    {
        _unitOfWork = unitOfWork;
        _outboxClient = outboxClient;
    }
    
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken cancellationToken)
    {
        var transaction = _unitOfWork.BeginTransaction();
        using var outboxTransaction = _outboxClient.UseTransaction(transaction);
        var response = await next();
        await outboxTransaction.CommitChangesAsync(cancellationToken);

        return response;
    }
}
