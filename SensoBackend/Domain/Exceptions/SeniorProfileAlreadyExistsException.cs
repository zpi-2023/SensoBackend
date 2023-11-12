namespace SensoBackend.Domain.Exceptions;

public class SeniorProfileAlreadyExistsException : Exception
{
    public SeniorProfileAlreadyExistsException() { }

    public SeniorProfileAlreadyExistsException(string message)
        : base(message) { }
}
