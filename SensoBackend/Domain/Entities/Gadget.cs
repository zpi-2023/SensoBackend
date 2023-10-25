namespace SensoBackend.Domain.Entities;

public sealed class Gadget
{
    public static readonly IReadOnlyList<Gadget> List = new List<Gadget>
    {
        new() { Id = 1, Name = "openMenu" },
        new() { Id = 2, Name = "switchProfile" },
        new() { Id = 3, Name = "logOut" },
        new() { Id = 4, Name = "activateSos" },
        new() { Id = 5, Name = "pairCaretaker" },
        new() { Id = 6, Name = "editDashboard" },
        new() { Id = 7, Name = "toggleLanguage" },
        new() { Id = 8, Name = "trackMedication" },
        new() { Id = 9, Name = "playGames" },
        new() { Id = 10, Name = "manageNotes" },
    };

    public required int Id { get; init; }

    public required string Name { get; init; }

    private Gadget() { }
}
