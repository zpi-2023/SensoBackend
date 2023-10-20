using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfileDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int SeniorId { get; init; }

    public string? Alias { get; init; }
}
