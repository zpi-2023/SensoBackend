namespace SensoBackend.Domain.Exceptions;

public class ProfileNotFoundException : Exception
{
    public ProfileNotFoundException(string message)
        : base(message) { }
}