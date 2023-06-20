using Supreme.Domain.Exceptions;

namespace Supreme.Api.Models.Common;

public class ErrorResponseModel
{
    public List<ErrorMessage> ErrorMessages { get; private set; } = new();

    public static ErrorResponseModel Create(List<ErrorMessage> errorMessages)
    {
        return new ErrorResponseModel { ErrorMessages = errorMessages };
    }
    
    public static ErrorResponseModel Create(string errorMessage)
    {
        return new ErrorResponseModel { ErrorMessages = new List<ErrorMessage>{new(errorMessage, null)} };
    }
}