using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class SeniorProfileAlreadyExistsException(int accountId)
    : ApiException(409, $"Account with id {accountId} already has a senior profile") { }
