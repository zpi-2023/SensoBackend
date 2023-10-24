using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class SeniorIdDto
{
    [Required]
    public required int SeniorId { get; init; }
}
