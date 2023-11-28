using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class ReminderNotFoundException(int reminderId)
    : ApiException(404, $"Reminder with id {reminderId} was not found") { }
