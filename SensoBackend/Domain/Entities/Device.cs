using System.ComponentModel.DataAnnotations;
using SensoBackend.Domain.Enums;

namespace SensoBackend.Domain.Entities;

public class Device
{
    public required int Id { get; set; }

    public required int AccountId { get; set; }

    public required string Token { get; set; }

    public required DeviceType Type { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset AddedAt { get; set; }

    public Account? Account { get; set; }
}
