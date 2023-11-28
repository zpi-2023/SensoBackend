using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class IntakeRecordNotFoundException(int intakeId)
    : ApiException(404, $"Intake record with id {intakeId} was not found") { }
