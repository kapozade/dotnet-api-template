using Supreme.Api.Models.Foo.Requests;
using Supreme.Api.Models.Foo.Responses;
using Supreme.Application.FooUseCases.Commands;
using Supreme.Application.FooUseCases.Queries;

namespace Supreme.Api.Mappers.Foo;

public static class FooMapper 
{
    public static GetFooQuery MapFrom(long id)
    {
        return new GetFooQuery(id);
    }

    public static GetFooResponse MapFrom(GetFooQueryResult result)
    {
        return new GetFooResponse(result.Id, result.Name);
    }

    public static CreateFooCommand MapFrom(CreateFooRequest request, int userId)
    {
        return new CreateFooCommand(request.Name, userId);
    }

    public static CreateFooResponse MapFrom(CreateFooCommandResult result)
    {
        return new CreateFooResponse(result.Id);
    }
}