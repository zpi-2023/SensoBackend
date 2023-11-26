namespace SensoBackend.Domain.Exceptions;

public class ReminderNotFoundException(int reminderId)
    : Exception($"Reminder with id {reminderId} was not found") { }
