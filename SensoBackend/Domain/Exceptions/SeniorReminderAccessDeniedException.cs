namespace SensoBackend.Domain.Exceptions;

public class SeniorReminderAccessDeniedException : Exception
{
    public SeniorReminderAccessDeniedException(int seniorId)
        : base($"You are not allowed to access or edit reminders of senior with id {seniorId}") { }
}