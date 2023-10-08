using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Domain.Entities;

public class Account
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required bool Active { get; init; }
    public required bool Verified { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DisplayName { get; init; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset CreatedAt { get; init; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset LastLoginAt { get; init; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset LastPasswordChangeAt { get; init; }
}
