using MediatR;

namespace Supreme.Domain.Outbox;

public interface ITransactionalRequest<out TReq> : IRequest<TReq> where TReq : class
{
}
