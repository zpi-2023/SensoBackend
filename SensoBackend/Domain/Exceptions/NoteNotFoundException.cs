namespace SensoBackend.Domain.Exceptions;

public class NoteNotFoundException(int noteId)
    : Exception($"Note with id {noteId} was not found") { }
