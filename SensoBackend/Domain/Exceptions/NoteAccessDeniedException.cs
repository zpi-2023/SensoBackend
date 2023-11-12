namespace SensoBackend.Domain.Exceptions;

public class NoteAccessDeniedException : Exception
{
    public NoteAccessDeniedException() { }

    public NoteAccessDeniedException(int noteId)
        : base($"You are not allowed to access the note with id {noteId}") { }
}
