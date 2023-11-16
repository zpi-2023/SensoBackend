namespace SensoBackend.Domain.Exceptions;

public class ReminderNotFoundException : Exception
{
    public ReminderNotFoundException(int reminderId)
        : base($"Reminder with id {reminderId} was not found") { }
}
