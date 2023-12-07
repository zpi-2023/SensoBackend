using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class AlertTypeNotFoundException(string alertTypeName)
    : ApiException(404, $"Alert type {alertTypeName} not found") { }
