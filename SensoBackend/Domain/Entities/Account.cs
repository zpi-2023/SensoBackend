using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Domain.Entities;

public sealed class Account
{
    public required int Id { get; init; }

    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool Active { get; set; }
    public required bool Verified { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayName { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset LastLoginAt { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset LastPasswordChangeAt { get; set; }
}
