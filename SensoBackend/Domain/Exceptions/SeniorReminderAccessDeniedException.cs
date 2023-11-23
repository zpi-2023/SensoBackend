namespace SensoBackend.Domain.Exceptions;

public class SeniorReminderAccessDeniedException(int seniorId)
    : Exception($"You are not allowed to access or edit reminders of senior with id {seniorId}") { }
