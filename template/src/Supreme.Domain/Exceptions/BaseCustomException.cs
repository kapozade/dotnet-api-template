namespace Supreme.Domain.Exceptions;

public abstract class BaseCustomException : Exception
{
    protected BaseCustomException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
