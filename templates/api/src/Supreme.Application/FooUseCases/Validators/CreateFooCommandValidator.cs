using Supreme.Application.FooUseCases.Commands;
using Supreme.Application.Validators;
using Supreme.Domain.Constants;
using FluentValidation;

namespace Supreme.Application.FooUseCases.Validators;

public class CreateFooCommandValidator : BaseNotNullableValidator<CreateFooCommand>
{
    public CreateFooCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessageKeys.FooMessages._nameRequired);

        RuleFor(x => x.Name)
            .MinimumLength(FieldConstants.FooFields._nameMinLength)
            .WithMessage(ValidationMessageKeys.FooMessages._nameLengthShouldBeGreaterThanOrEqualTo)
            .WithState(_ => FieldConstants.FooFields._nameMinLength)
            .MaximumLength(FieldConstants.FooFields._nameMaxLength)
            .WithMessage(ValidationMessageKeys.FooMessages._nameLengthShouldBeLessThanOrEqualTo)
            .WithState(_ => FieldConstants.FooFields._nameMaxLength);

        RuleFor(x => x.ExecutedBy)
            .GreaterThan(FieldConstants._userIdMinValue)
            .WithMessage(ValidationMessageKeys._userIdShouldBeGreaterThan)
            .WithState(_ => FieldConstants._userIdMinValue);
    }
}
