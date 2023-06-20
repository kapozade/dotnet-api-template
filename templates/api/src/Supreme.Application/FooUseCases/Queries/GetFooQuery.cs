using MediatR;

namespace Supreme.Application.FooUseCases.Queries;

public record GetFooQuery(long Id) : IRequest<GetFooQueryResult>;

public record GetFooQueryResult(long Id, string Name);