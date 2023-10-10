namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record AccountDto
{
    public required int Id { get; init; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool Active { get; set; }
    public required bool Verified { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset LastLoginAt { get; set; }
    public required DateTimeOffset LastPasswordChangeAt { get; set; }
}
