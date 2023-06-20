namespace Supreme.Domain.Exceptions;

public sealed class ValidationException : BaseCustomException
{
    public List<ErrorMessage> ValidationErrors { get; }

    public ValidationException(List<ErrorMessage> validationErrors) : base(string.Empty)
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string message) : base(message)
    {
        ValidationErrors = new List<ErrorMessage> { new(message, null) };
    }

    public ValidationException(string message, object data) : base(message)
    {
        ValidationErrors = new List<ErrorMessage> { new(message, data) };
    }
}

public record ErrorMessage(string Message, object? Data);

public record CustomAttemptedValueForValidationError(object? Data);
