using Microsoft.AspNetCore.Mvc;

namespace SensoBackend.Application.Modules.Pagination;

public sealed record PaginationQuery
{
    [FromQuery(Name = "limit")]
    public int Limit { get; init; } //TODO: add checks

    [FromQuery(Name = "page")]
    public int Page { get; init; }
}
