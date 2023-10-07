using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SensoBackend.WebApi.Middlewares;

namespace SensoBackend.Tests.WebApi.Middlewares;

public sealed class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock = new();
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock = new();
    private readonly ExceptionMiddleware _sut;

    public ExceptionMiddlewareTests() =>
        _sut = new ExceptionMiddleware(_nextMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Invoke_ShouldCallNextDelegateInChain()
    {
        _nextMock.Setup(s => s(It.IsAny<HttpContext>()));

        await _sut.Invoke(new DefaultHttpContext());

        _nextMock.Verify(s => s(It.IsAny<HttpContext>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_ShouldSetBadRequestStatusCode_WhenValidationErrorOccurred()
    {
        _nextMock
            .Setup(s => s(It.IsAny<HttpContext>()))
            .ThrowsAsync(new ValidationException(new List<ValidationFailure>()));

        var context = new DefaultHttpContext();
        await _sut.Invoke(context);

        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Invoke_ShouldSetInternalErrorStatusCode_WhenUnknownErrorOccurred()
    {
        _nextMock.Setup(s => s(It.IsAny<HttpContext>())).ThrowsAsync(new Exception());

        var context = new DefaultHttpContext();
        await _sut.Invoke(context);

        context.Response.StatusCode.Should().Be(500);
    }
}
