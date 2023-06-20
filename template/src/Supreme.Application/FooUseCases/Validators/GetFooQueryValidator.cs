using Supreme.Application.FooUseCases.Queries;
using Supreme.Application.Validators;
using Supreme.Domain.Constants;
using FluentValidation;

namespace Supreme.Application.FooUseCases.Validators;

public class GetFooQueryValidator : BaseNotNullableValidator<GetFooQuery>
{
    public GetFooQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(FieldConstants.FooFields._idMinValue)
            .WithMessage(ValidationMessageKeys.FooMessages._idShouldBeGreaterThan)
            .WithState(_ => FieldConstants.FooFields._idMinValue);
    }
}
