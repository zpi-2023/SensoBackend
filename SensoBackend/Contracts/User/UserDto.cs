using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Contracts.User;

public record UserDto
{
    [Required]
    public required int Id { get; set; }

    [Required] public required string Name { get; set; }
}
