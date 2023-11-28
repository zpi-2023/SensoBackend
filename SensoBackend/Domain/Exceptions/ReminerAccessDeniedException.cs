using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class ReminderAccessDeniedException(int reminderId)
    : ApiException(403, $"Access to the reminder with id {reminderId} was denied") { }
