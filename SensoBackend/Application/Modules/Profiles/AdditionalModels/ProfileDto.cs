using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.AdditionalModels;

public class ProfileDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int SeniorId { get; init; }

    [Required]
    public required string Alias { get; init; }
}
