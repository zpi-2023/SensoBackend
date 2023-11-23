namespace SensoBackend.Domain.Exceptions;

public class ReminderAccessDeniedException(int reminderId)
    : Exception($"Access to the reminder with id {reminderId} was denied") { }
