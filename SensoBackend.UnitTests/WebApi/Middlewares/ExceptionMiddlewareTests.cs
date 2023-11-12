using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using SensoBackend.WebApi.Middlewares;
using System.Security.Authentication;
using SensoBackend.Domain.Exceptions;

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
    public async Task Invoke_ShouldSetBadRequestStatusCode_WhenProfileNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new ProfileNotFoundException(""));

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(400);
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
    public async Task Invoke_ShouldSetUnauthorizedStatusCode_WhenInvalidCredentialExceptionOccurred2()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new AuthenticationException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Invoke_ShouldSetForbiddenStatusCode_WhenNoteAccessDeniedExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new NoteAccessDeniedException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task Invoke_ShouldSetForbiddenStatusCode_WhenRemoveSeniorProfileDeniedExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new RemoveSeniorProfileDeniedException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenAccountNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new AccountNotFoundException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenNoteNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new NoteNotFoundException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetNotFoundStatusCode_WhenSeniorNotFoundExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new SeniorNotFoundException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Invoke_ShouldSetConflictStatusCode_WhenCaretakerProfileAlreadyExistsExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new CaretakerProfileAlreadyExistsException());

        await _sut.Invoke(context);

        context.Response.Should().NotBeNull();
        context.Response.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task Invoke_ShouldSetConflictStatusCode_WhenSeniorProfileAlreadyExistsExceptionOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new SeniorProfileAlreadyExistsException());

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
