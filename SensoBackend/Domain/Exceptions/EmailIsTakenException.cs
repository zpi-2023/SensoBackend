using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class EmailIsTakenException(string email)
    : ApiException(409, $"Email {email} is already taken") { }
