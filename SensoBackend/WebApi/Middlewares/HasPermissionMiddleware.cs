using Microsoft.AspNetCore.Http.Features;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Enums;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using System.Net;
using System.Security.Claims;

namespace SensoBackend.WebApi.Middlewares;

public class HasPermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HasPermissionMiddleware> _logger;

    public HasPermissionMiddleware(RequestDelegate next, ILogger<HasPermissionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<HasPermissionAttribute>();
        if (attribute == null)
        {
            await _next(context);
            return;
        }

        _logger.LogInformation(
            "HasPermissionAttribute found, validating permission {Permission}...",
            attribute.Permission
        );

        var accountIdString = context.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (accountIdString == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        var accountId = int.Parse(accountIdString);
        var role = await authorizationService.GetRoleAsync(accountId);

        _logger.LogInformation("Extracted account: {Role} with id {AccountId}", role, accountId);

        if (role == Role.Admin)
        {
            await _next(context);
            return;
        }

        if (role != Role.Member)
        {
            throw new InvalidDataException($"The role {role} not supported");
        }

        var permissions = Role.Member.GetPermissions();
        if (!permissions.Contains(attribute.Permission))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        var profiles = await authorizationService.GetProfilesByAccountId(accountId);
        var seniorIdStr = context.Request.RouteValues["seniorId"]?.ToString();

        if (seniorIdStr == null)
        {
            await _next(context);
            return;
        }

        _logger.LogInformation("Path contains the seniorId param, looking for a valid profile...");

        var isIdValid = int.TryParse(seniorIdStr, out var seniorId);
        if (!isIdValid)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        var seniorExists = profiles.Where(p => p.SeniorId == seniorId).Any();

        if (!seniorExists)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(context);
    }
}
