namespace SensoBackend.Application.Modules.Pagination.Contracts;

public sealed record PaginatedDto<T>
{
    public required List<T> Items { get; init; }

    public required int CurrentPage { get; init; }
}
