namespace SensoBackend.Domain.Exceptions;

public class NoteAccessDeniedException(int noteId)
    : Exception($"You are not allowed to access the note with id {noteId}") { }
