using System.Collections;
using System.Security.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using SensoBackend.Domain.Exceptions;
using SensoBackend.WebApi.Middlewares;

namespace SensoBackend.Tests.WebApi.Middlewares;

public sealed class ExceptionMiddlewareTests
{
    private readonly RequestDelegate _next = Substitute.For<RequestDelegate>();
    private readonly ILogger<ExceptionMiddleware> _logger = Substitute.For<
        ILogger<ExceptionMiddleware>
    >();
    private readonly ExceptionMiddleware _sut;

    public ExceptionMiddlewareTests() => _sut = new ExceptionMiddleware(_next, _logger);

    private Exception? CreateExceptionOfType(Type exceptionType)
    {
        var paramsInfo = exceptionType.GetConstructors().First().GetParameters();
        var paramsList = new List<object>();
        foreach (var param in paramsInfo)
        {
            paramsList.Add(
                param.ParameterType switch
                {
                    var t when t == typeof(int) => 0,
                    var t when t == typeof(string) => string.Empty,
                    var t when t == typeof(IEnumerable) => new List<object>(),
                    _
                        => throw new TypeInitializationException(
                            exceptionType.FullName,
                            new Exception(
                                $"Type {exceptionType.FullName} has unsupported constructor parameter type {param.ParameterType.FullName}"
                            )
                        )
                }
            );
        }

        return Activator.CreateInstance(exceptionType, args: [.. paramsList]) as Exception;
    }

    [Fact]
    public async Task Invoke_ShouldCallNextDelegateInChain()
    {
        await _sut.Invoke(new DefaultHttpContext());

        _next.ReceivedWithAnyArgs();
    }

    [Fact]
    public async Task Invoke_ShouldSetInternalErrorStatusCode_WhenUnknownExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new Exception());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(500);
    }

    [Theory]
    [InlineData(400, typeof(ValidationException))]
    [InlineData(401, typeof(InvalidCredentialException))]
    [InlineData(403, typeof(NoteAccessDeniedException))]
    [InlineData(403, typeof(ReminderAccessDeniedException))]
    [InlineData(403, typeof(ReminderNotActiveException))]
    [InlineData(403, typeof(RemoveSeniorProfileDeniedException))]
    [InlineData(403, typeof(SeniorReminderAccessDeniedException))]
    [InlineData(404, typeof(AlertTypeNotFoundException))]
    [InlineData(404, typeof(GameNotFoundException))]
    [InlineData(404, typeof(HashNotFoundException))]
    [InlineData(404, typeof(IntakeRecordNotFoundException))]
    [InlineData(404, typeof(NoteNotFoundException))]
    [InlineData(404, typeof(ProfileNotFoundException))]
    [InlineData(404, typeof(ReminderNotFoundException))]
    [InlineData(404, typeof(SeniorNotFoundException))]
    [InlineData(409, typeof(CaretakerProfileAlreadyExistsException))]
    [InlineData(409, typeof(EmailIsTakenException))]
    [InlineData(409, typeof(SeniorProfileAlreadyExistsException))]
    public async Task Invoke_ShouldSetGivenStatusCode_WhenGivenExceptionOccured(
        int statusCode,
        Type exceptionType
    )
    {
        var exception = CreateExceptionOfType(exceptionType);

        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(exception);

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(statusCode);
    }
}
