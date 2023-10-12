using FluentValidation;
using Microsoft.Extensions.Primitives;
using System.Security.Authentication;

namespace SensoBackend.WebApi.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error during request processing.");
            context.Response.ContentType = "application/json";
            var noControlCharsExceptionMessage = new string(
                exception.Message.Where(c => !char.IsControl(c)).ToArray()
            );
            context.Response.Headers.Add("X-Error-Message", noControlCharsExceptionMessage);
            context.Response.StatusCode = exception switch
            {
                ValidationException => 400,
                InvalidCredentialException => 401,
                _ => 500
            };
        }
    }
}
