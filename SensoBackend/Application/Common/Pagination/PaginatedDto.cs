namespace SensoBackend.Application.Common.Pagination;

public sealed record PaginatedDto<T>
{
    public required List<T> Items { get; init; }
}
