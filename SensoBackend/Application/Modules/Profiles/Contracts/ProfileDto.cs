using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfileDto
{
    [Required]
    public required int Id { get; set; }

    [Required]
    public required int AccountId { get; set; }

    [Required]
    public required int SeniorId { get; set; }

    public string? Alias { get; set; }
}
