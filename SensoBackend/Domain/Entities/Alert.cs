using System.ComponentModel.DataAnnotations;
using SensoBackend.Domain.Enums;

namespace SensoBackend.Domain.Entities;

public class Alert
{
    public required int Id { get; set; }

    public required int SeniorId { get; set; }

    public required AlertType Type { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset FiredAt { get; set; }

    public Account? Senior { get; set; }
}
