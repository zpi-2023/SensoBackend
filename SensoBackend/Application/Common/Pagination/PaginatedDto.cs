using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Common.Pagination;

public sealed record PaginatedDto<T>
{
    [Required]
    public required List<T> Items { get; init; }
}
