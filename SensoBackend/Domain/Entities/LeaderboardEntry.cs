using SensoBackend.Domain.Enums;

namespace SensoBackend.Domain.Entities;

public class LeaderboardEntry
{
    public required int Id { get; set; }

    public required int AccountId { get; set; }

    public required Game Game { get; set; }

    public required int Score { get; set; }

    public Account? Account { get; set; }
}
