using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class ProfileNotFoundException(int accountId, int seniorId)
    : ApiException(
        404,
        $"Profile with AccountId {accountId} and SeniorId {seniorId} was not found"
    ) { }
