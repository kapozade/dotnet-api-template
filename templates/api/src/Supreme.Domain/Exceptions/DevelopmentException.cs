namespace Supreme.Domain.Exceptions;

public sealed class DevelopmentException : BaseCustomException
{
    public DevelopmentException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
