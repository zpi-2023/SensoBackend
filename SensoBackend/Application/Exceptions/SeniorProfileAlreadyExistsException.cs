namespace SensoBackend.Application.Exceptions;

public class SeniorProfileAlreadyExistsException : Exception
{
    public SeniorProfileAlreadyExistsException(string message)
        : base(message) { }
}
