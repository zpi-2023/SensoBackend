using FluentValidation;
using SensoBackend.Domain.Exceptions;
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
                SeniorReminderAccessDeniedException => 403,
                ReminderAccessDeniedException => 403,
                NoteAccessDeniedException => 403,
                RemoveSeniorProfileDeniedException => 403,
                NoteNotFoundException => 404,
                ProfileNotFoundException => 404,
                SeniorNotFoundException => 404,
                NoteNotFoundException => 404,
                ReminderNotFoundException => 404,
                IntakeRecordNotFoundException => 404,
                CaretakerProfileAlreadyExistsException => 409,
                SeniorProfileAlreadyExistsException => 409,
                _ => 500
            };
        }
    }
}
