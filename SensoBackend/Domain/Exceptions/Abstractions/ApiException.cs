namespace SensoBackend.Domain.Exceptions.Abstractions;

public abstract class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
