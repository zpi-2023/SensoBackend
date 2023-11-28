using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class SeniorReminderAccessDeniedException(int seniorId)
    : ApiException(
        403,
        $"You are not allowed to access or edit reminders of senior with id {seniorId}"
    ) { }
