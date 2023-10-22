using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfileDisplayDto
{
    [Required]
    public string Type { get; set; }

    [Required]
    public int SeniorId { get; set; }

    public string? SeniorAlias { get; set; }

    internal ProfileDisplayDto(ProfileDto profile)
    {
        Type = profile.AccountId == profile.SeniorId
            ? "senior"
            : "caretaker";
        SeniorId = profile.SeniorId;
        SeniorAlias = profile.Alias;
    }
}
