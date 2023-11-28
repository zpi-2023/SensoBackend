using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class SeniorNotFoundException(int seniorId)
    : ApiException(404, $"A senior with Id {seniorId} does not exist") { }
