using Microsoft.AspNetCore.Http.Features;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Entities;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using System.Net;
using System.Security.Claims;

namespace SensoBackend.WebApi.Middlewares;

public class HasPermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<HasPermissionMiddleware> _logger;

    public HasPermissionMiddleware(
        RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<HasPermissionMiddleware> logger
    )
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<HasPermissionAttribute>();
        if (attribute == null)
        {
            await _next(context);
            return;
        }

        var accountIdString = context.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (accountIdString == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IAuthorizationService authorizationService =
            scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var accountId = int.Parse(accountIdString!);
        var roleId = await authorizationService.GetRoleIdAsync(accountId);

        if (roleId == Role.AdminId)
        {
            await _next(context);
            return;
        }

        if (roleId != Role.MemberId)
        {
            throw new InvalidDataException(
                $"The role with id {roleId} does not exist or is not supported"
            );
        }

        var requiredPermission = attribute.Permission;
        var permissions = Role.Member.GetPermissions();
        if (!permissions.Contains(requiredPermission))
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
