namespace SensoBackend.Domain.Exceptions;

public class EmailIsTakenException(string email) : Exception($"Email {email} is already taken") { }
