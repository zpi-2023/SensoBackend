namespace SensoBackend.Application.Common.Pagination;

public static class PageExtension
{
    public static IQueryable<T> Paged<T>(this IQueryable<T> source, PaginationQuery query) =>
        source.Skip(query.Offset).Take(query.Limit);
}
