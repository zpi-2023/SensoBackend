namespace SensoBackend.Domain.Exceptions;

public class RemoveSeniorProfileDeniedException : Exception
{
    public RemoveSeniorProfileDeniedException(string message)
        : base(message) { }
}
