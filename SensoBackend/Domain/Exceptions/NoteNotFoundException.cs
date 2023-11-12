namespace SensoBackend.Domain.Exceptions;

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException() { }

    public NoteNotFoundException(int noteId)
        : base($"Note with id {noteId} was not found") { }
}
