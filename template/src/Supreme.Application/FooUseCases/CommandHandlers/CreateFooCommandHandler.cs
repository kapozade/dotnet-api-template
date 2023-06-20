using Supreme.Application.FooUseCases.Commands;
using Supreme.Domain.Entities.Foo;
using Supreme.Domain.Factories;
using Supreme.Domain.Db;
using MediatR;

namespace Supreme.Application.FooUseCases.CommandHandlers;

internal sealed class CreateFooCommandHandler : IRequestHandler<CreateFooCommand, CreateFooCommandResult>
{
   private readonly IUnitOfWork _unitOfWork;

    public CreateFooCommandHandler(
        IUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFooCommandResult> Handle(CreateFooCommand request, CancellationToken cancellationToken)
    {
        var foo = Foo.Create(request.Name, request.ExecutedBy, DateTimeFactory.Now());
        await _unitOfWork.FooRepository.AddAsync(foo, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateFooCommandResult(foo.Id);
    }
}
