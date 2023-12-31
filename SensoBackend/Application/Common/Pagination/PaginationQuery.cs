﻿using Microsoft.AspNetCore.Mvc;

namespace SensoBackend.Application.Common.Pagination;

public sealed record PaginationQuery
{
    [FromQuery(Name = "offset")]
    public required int Offset { get; init; } = 0;

    [FromQuery(Name = "limit")]
    public required int Limit { get; init; } = 5;
}
