using System.Security.Authentication;
using FluentValidation;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.WebApi.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error during request processing.");
            context.Response.ContentType = "application/json";
            var noControlCharsExceptionMessage = new string(
                exception.Message.Where(c => !char.IsControl(c)).ToArray()
            );
            context.Response.Headers.Append("X-Error-Message", noControlCharsExceptionMessage);
            context.Response.StatusCode = exception switch
            {
                ApiException apiException => apiException.StatusCode,
                ValidationException => 400,
                InvalidCredentialException => 401,
                _ => 500
            };
        }
    }
}
