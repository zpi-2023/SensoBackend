using Microsoft.AspNetCore.Http.Features;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Enums;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace SensoBackend.WebApi.Middlewares;

public enum HasPermissionResult
{
    Next,
    BlockWithUnauthorized,
    BlockWithBadRequest
}

public class HasPermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HasPermissionMiddleware> _logger;

    public HasPermissionMiddleware(RequestDelegate next, ILogger<HasPermissionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task<HasPermissionResult> ValidateAsync(
        IAuthorizationService authorizationService,
        HasPermissionAttribute? attribute,
        string? seniorIdParam,
        string? accountIdClaim
    )
    {
        if (attribute is null)
        {
            return HasPermissionResult.Next;
        }

        _logger.LogInformation(
            "HasPermissionAttribute found, validating permission {Permission}...",
            attribute.Permission
        );

        if (accountIdClaim is null || !int.TryParse(accountIdClaim, out var accountId))
        {
            return HasPermissionResult.BlockWithUnauthorized;
        }

        var role = await authorizationService.GetRoleAsync(accountId);

        _logger.LogInformation("Extracted account: {Role} with id {AccountId}", role, accountId);

        if (role == Role.Admin)
        {
            return HasPermissionResult.Next;
        }

        if (role != Role.Member)
        {
            throw new InvalidDataException($"The role {role} not supported");
        }

        if (!role.GetPermissions().Contains(attribute.Permission))
        {
            return HasPermissionResult.BlockWithUnauthorized;
        }

        if (seniorIdParam is null)
        {
            return HasPermissionResult.Next;
        }

        _logger.LogInformation("Path contains the seniorId param, looking for a valid profile...");

        if (!int.TryParse(seniorIdParam, out var seniorId))
        {
            return HasPermissionResult.BlockWithBadRequest;
        }

        var profiles = await authorizationService.GetProfilesByAccountId(accountId);
        if (!profiles.Any(p => p.SeniorId == seniorId))
        {
            return HasPermissionResult.BlockWithUnauthorized;
        }

        return HasPermissionResult.Next;
    }

    public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
    {
        _logger.LogInformation("Processing the request through the HasPermissionMiddleware...");

        var attribute = context.Features
            .Get<IEndpointFeature>()
            ?.Endpoint?.Metadata.GetMetadata<HasPermissionAttribute>();
        var seniorIdParam = context.Request.RouteValues["seniorId"]?.ToString();
        var accountIdClaim = context.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        var result = await ValidateAsync(
            authorizationService,
            attribute,
            seniorIdParam,
            accountIdClaim
        );

        switch (result)
        {
            case HasPermissionResult.Next:
                _logger.LogInformation("Permission granted, continuing!");
                await _next(context);
                return;
            case HasPermissionResult.BlockWithUnauthorized:
                _logger.LogInformation("Permission denied, blocking with 401 Unauthorized!");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            case HasPermissionResult.BlockWithBadRequest:
                _logger.LogInformation("Permission denied, blocking with 400 Bad Request!");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            default:
                throw new UnreachableException();
        }
    }
}
