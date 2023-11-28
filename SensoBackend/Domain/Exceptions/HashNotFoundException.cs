using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class HashNotFoundException()
    : ApiException(404, "Provided hash was not found in the database") { }
