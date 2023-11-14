using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Games.Contracts;

public sealed record LeaderboardEntryDto
{
    [Required]
    public required string DisplayName { get; init; }

    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int Score { get; init; }
}
