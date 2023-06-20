using Supreme.Api.Models.Common;
using Supreme.Domain.Exceptions;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Supreme.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(
        RequestDelegate next
        )
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, ILogger<GlobalExceptionMiddleware> logger)
    {
        try
        {
            httpContext.Request.EnableBuffering();
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex is not BaseCustomException)
            {
                var requestAsText = await RequestToString(httpContext);
                logger.LogError(ex, "{RequestAsText}{NewLine1}{ExceptionMessage}", requestAsText,
                    Environment.NewLine, ex.Message);
            }
            await HandleErrorAsync(ex, httpContext);
        }
    }

    private async Task HandleErrorAsync(Exception exception, HttpContext context)
    {
        context.Response.StatusCode = GetStatusCode(exception);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(GetPayload(exception)));
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (int) HttpStatusCode.NotFound,
            ValidationException => (int) HttpStatusCode.BadRequest,
            _ => (int) HttpStatusCode.InternalServerError
        };
    }

    private static ErrorResponseModel GetPayload(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => ErrorResponseModel.Create(validationException.ValidationErrors),
            NotFoundException notFoundException => ErrorResponseModel.Create(notFoundException.Message),
            _ => ErrorResponseModel.Create("Unexpected error occured.")
        };
    }

    private static async Task<string> RequestToString(HttpContext httpContext)
    {
        var rawRequestBody = await GetRawBodyAsync(httpContext.Request);

        var headerLine = httpContext.Request
            .Headers
            .Where(h => h.Key != "Authentication")
            .Select(pair => $"{pair.Key} => {string.Join("|", pair.Value.ToList())}");

        var headerText = string.Join(Environment.NewLine, headerLine);

        var message =
            $"Request: {httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}{Environment.NewLine}" +
            $"Headers: {Environment.NewLine}{headerText}{Environment.NewLine}" +
            $"Content: {Environment.NewLine}{rawRequestBody}";

        return message;
    }

    private static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }
}
