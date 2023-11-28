using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class RemoveSeniorProfileDeniedException()
    : ApiException(403, "This senior profile has caretaker profiles associated with it") { }
