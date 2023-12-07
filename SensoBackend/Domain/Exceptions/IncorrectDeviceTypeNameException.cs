using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class IncorrectDeviceTypeNameException(string deviceTypeName)
    : ApiException(400, $"Device type {deviceTypeName} is incorrect") { }
