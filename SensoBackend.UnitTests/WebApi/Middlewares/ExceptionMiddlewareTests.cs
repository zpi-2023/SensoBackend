using System.Security.Authentication;
using FluentValidation;
using FluentValidation.Results;
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

    [Fact]
    public async Task Invoke_ShouldCallNextDelegateInChain()
    {
        await _sut.Invoke(new DefaultHttpContext());

        _next.ReceivedWithAnyArgs();
    }

    [Fact]
    public async Task Invoke_ShouldSetBadRequestStatusCode_WhenValidationExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new ValidationException(new List<ValidationFailure>()));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Invoke_ShouldSetUnauthorizedStatusCode_WhenInvalidCredentialExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new InvalidCredentialException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Invoke_ShouldSetForbiddenStatusCode_WhenNoteAccessDeniedExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new NoteAccessDeniedException(0));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task Invoke_ShouldSetForbiddenStatusCode_WhenRemoveSeniorProfileDeniedExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new RemoveSeniorProfileDeniedException(String.Empty));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenGameNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new GameNotFoundException(string.Empty));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenNoteNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new NoteNotFoundException(0));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetBadRequestStatusCode_WhenProfileNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new ProfileNotFoundException(""));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenSeniorNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new SeniorNotFoundException(String.Empty));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetConflictStatusCode_WhenCaretakerProfileAlreadyExistsExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new CaretakerProfileAlreadyExistsException(String.Empty));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task Invoke_ShouldSetConflictStatusCode_WhenSeniorProfileAlreadyExistsExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new SeniorProfileAlreadyExistsException(String.Empty));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(409);
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
}
