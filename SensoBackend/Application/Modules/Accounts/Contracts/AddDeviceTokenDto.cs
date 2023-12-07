using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record AddDeviceTokenDto
{
    [Required]
    public required string DeviceToken { get; init; }

    [Required]
    public required string DeviceType { get; init; }
}
