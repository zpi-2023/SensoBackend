namespace SensoBackend.Domain.Exceptions;

public class CaretakerProfileAlreadyExistsException : Exception
{
    public CaretakerProfileAlreadyExistsException(string message)
        : base(message) { }
}
