using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class EncodedSeniorIdDto
{
    [Required]
    public required string EncodedSeniorId { get; init; }
}
