namespace SensoBackend.Domain.Exceptions;

public class ReminderAccessDeniedException : Exception
{
    public ReminderAccessDeniedException(int reminderId)
        : base($"Access to the reminder with id {reminderId} was denied") { }
}
