using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class CaretakerProfileAlreadyExistsException()
    : ApiException(409, "This caretaker profile already exists") { }
