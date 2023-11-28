using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class NoteAccessDeniedException(int noteId)
    : ApiException(403, $"You are not allowed to access the note with id {noteId}") { }
