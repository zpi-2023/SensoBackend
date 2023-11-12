namespace SensoBackend.Domain.Exceptions;

public class IntakeRecordNotFoundException : Exception
{
    public IntakeRecordNotFoundException(int intakeId)
        : base($"Intake record with id {intakeId} was not found") { }
}
