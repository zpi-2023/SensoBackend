using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class NoteNotFoundException(int noteId)
    : ApiException(404, $"Note with id {noteId} was not found") { }
