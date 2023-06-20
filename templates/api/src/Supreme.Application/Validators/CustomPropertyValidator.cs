using FluentValidation;
using FluentValidation.Validators;

namespace Supreme.Application.Validators;

// Example
public class CustomPropertyValidator<T> : PropertyValidator<T, decimal?>
{
    public override string Name { get => "CustomProperty"; }

    public override bool IsValid(ValidationContext<T> context, decimal? value)
    {
        return value == null || (value is >= -90 and <= 90);
    }
}

public static class CustomPropertyValidatorExtension
{
    public static IRuleBuilderOptions<T, decimal?> IsCustomPropertySatisfied<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new CustomPropertyValidator<T>());
    }
}
