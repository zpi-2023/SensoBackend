using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Contracts.User;

public record CreateUserDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }
}