using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
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
    public async Task Invoke_ShouldSetBadRequestStatusCode_WhenValidationErrorOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new ValidationException(new List<ValidationFailure>()));

        await _sut.Invoke(context);

        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Invoke_ShouldSetInternalErrorStatusCode_WhenUnknownErrorOccurred()
    {
        var context = new DefaultHttpContext();
        _next.Invoke(context).Throws(new Exception());

        await _sut.Invoke(context);

        context.Response.StatusCode.Should().Be(500);
    }
}
