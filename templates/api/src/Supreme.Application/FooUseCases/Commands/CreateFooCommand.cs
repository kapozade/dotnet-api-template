#if (enable-outbox-pattern)
using Supreme.Domain.Outbox;
#endif
#if (!enable-outbox-pattern)
using MediatR;
#endif

namespace Supreme.Application.FooUseCases.Commands;

#if (enable-outbox-pattern)
public record CreateFooCommand(string Name, int ExecutedBy) : ITransactionalRequest<CreateFooCommandResult>;
#endif
#if (!enable-outbox-pattern)
public record CreateFooCommand(string Name, int ExecutedBy) : IRequest<CreateFooCommandResult>;
#endif
public record CreateFooCommandResult(long Id);