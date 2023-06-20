using Supreme.Domain.Exceptions;
using FluentValidation;
using MediatR;
using ValidationException = Supreme.Domain.Exceptions.ValidationException;

namespace Supreme.Infrastructure.PipelineBehaviors;

public class ValidationPipelineBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TReq: IRequest<TRes>
{
    private readonly IEnumerable<IValidator<TReq>> _validators;

    public ValidationPipelineBehavior(
        IEnumerable<IValidator<TReq>> validators
        )
    {
        _validators = validators;
    }

    public Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken cancellationToken)
    {
        var validationFailures
            = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(validationResult => validationResult.Errors)
                .Where(validationFailure => validationFailure != null)
                .ToList();

        if (!validationFailures.Any()) return next();
        
        var errors = new List<ErrorMessage>();
        foreach (var validationFailure in validationFailures)
        {
            object? data = null;
            if (validationFailure.CustomState != null)
            {
                data = validationFailure.CustomState;
            }
            else
            {
                if (validationFailure.AttemptedValue != null)
                {
                    var type = validationFailure.AttemptedValue.GetType();
                    if (type == typeof(CustomAttemptedValueForValidationError)) 
                        data = (validationFailure.AttemptedValue as CustomAttemptedValueForValidationError)?.Data;
                }
            }
            
            errors.Add(new ErrorMessage(validationFailure.ErrorMessage, data));
        }
                
        throw new ValidationException(errors);
    }
}
