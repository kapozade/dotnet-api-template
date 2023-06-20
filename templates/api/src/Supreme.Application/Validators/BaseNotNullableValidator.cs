using Supreme.Domain.Constants;
using FluentValidation;
using FluentValidation.Results;

namespace Supreme.Application.Validators;

public class BaseNotNullableValidator<T>: AbstractValidator<T>
{
    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
    {
        if (context.InstanceToValidate != null)
        {
            return true;
        }

        result.Errors.Add(new ValidationFailure(string.Empty, ValidationMessageKeys._requestBodyRequired));
        return false;
    }
}
