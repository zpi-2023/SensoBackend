namespace SensoBackend.Domain.Exceptions;

public class SeniorNotFoundException : Exception
{
    public SeniorNotFoundException(string message)
        : base(message) { }
}
