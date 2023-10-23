namespace SensoBackend.Application.Exceptions;

public class CaretakerProfileAlreadyExistsException : Exception
{
    public CaretakerProfileAlreadyExistsException(string message) 
        :base(message) { }
}
