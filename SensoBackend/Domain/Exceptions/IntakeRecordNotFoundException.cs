namespace SensoBackend.Domain.Exceptions;

public class IntakeRecordNotFoundException(int intakeId)
    : Exception($"Intake record with id {intakeId} was not found") { }
