using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Games.Contracts;

public sealed record ScoreDto
{
    [Required]
    public int Score { get; init; }
}
