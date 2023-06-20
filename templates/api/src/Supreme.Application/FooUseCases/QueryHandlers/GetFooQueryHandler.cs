using  Supreme.Application.FooUseCases.Queries;
using  Supreme.Domain.Db;
using Supreme.Domain.Entities.Foo.Specifications;
using MediatR;

namespace Supreme.Application.FooUseCases.QueryHandlers;

internal sealed class GetFooQueryHandler : IRequestHandler<GetFooQuery, GetFooQueryResult>
{
   private readonly IUnitOfWork _unitOfWork;

    public GetFooQueryHandler(
        IUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetFooQueryResult> Handle(GetFooQuery request, CancellationToken cancellationToken)
    {
        // This is an example for specification pattern. You can ofcourse directly call GetById method of the repository.
        var byIdSpecification = new ByIdSpec(request.Id, false);
        var result = await _unitOfWork.FooRepository.GetAsync(byIdSpecification, cancellationToken);
        var response = new GetFooQueryResult(result.Id, result.Name);

        return response;
    }
}
