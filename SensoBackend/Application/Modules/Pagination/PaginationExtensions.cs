namespace SensoBackend.Application.Modules.Pagination;

public static class PaginationExtensions
{
    public static IQueryable<T> Paged<T>(this IQueryable<T> source, PaginationQuery query) =>
        source.Skip(query.Page * query.Limit).Take(query.Limit);
}
