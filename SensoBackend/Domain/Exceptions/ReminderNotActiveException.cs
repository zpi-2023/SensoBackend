namespace SensoBackend.Domain.Exceptions;

public class ReminderNotActiveException(int reminderId)
    : Exception($"Reminder with id {reminderId} is no longer active") { }
