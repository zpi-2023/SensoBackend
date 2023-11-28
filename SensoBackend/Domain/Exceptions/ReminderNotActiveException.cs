using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class ReminderNotActiveException(int reminderId)
    : ApiException(403, $"Reminder with id {reminderId} is no longer active") { }
