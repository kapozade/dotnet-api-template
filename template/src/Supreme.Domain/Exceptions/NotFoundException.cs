namespace Supreme.Domain.Exceptions;

public sealed class NotFoundException : BaseCustomException
{
    public NotFoundException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
