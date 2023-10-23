using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.AdditionalModels;

public class GetEncodedSeniorIdDto
{
    [Required]
    public required int AccountId { get; init; }
}
