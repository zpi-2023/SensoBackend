namespace SensoBackend.Application.Exceptions;

public class SeniorNotFoundException : Exception
{
    public SeniorNotFoundException(string message)
        : base(message) { }
}
