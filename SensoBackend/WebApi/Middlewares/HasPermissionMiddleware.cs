using Microsoft.AspNetCore.Http.Features;
using SensoBackend.Application.Abstractions;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using System.Net;
using System.Security.Claims;

namespace SensoBackend.WebApi.Middlewares;

public class HasPermissionMiddleware
{
    private RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<HasPermissionMiddleware> _logger; //may be useful later on

    public HasPermissionMiddleware(
        RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<HasPermissionMiddleware> logger)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        //0. Check if attribute present
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<HasPermissionAttribute>();
        if (attribute != null)
        {
            //1. Check JWT token
            var accountIdString = context.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            if (accountIdString == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            //1.5 Get the role
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IAuthorizationService authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
            var accountId = int.Parse(accountIdString!);
            var roleId = await authorizationService.GetRoleIdAsync(accountId);

            //2. Check if admin
            if (roleId == (int)Role.Admin)
            {
                await _next(context);
                return;
            }

            //2.5 Check permissions
            var requiredPermission = attribute.Permission;
            if (roleId == (int)Role.Member)
            {
                var permissions = Role.Member.GetPermissions();
                if (permissions.Contains(requiredPermission))
                {
                    //3. Get profiles for accountId
                    var profiles = await authorizationService.GetProfilesByAccountId(accountId);

                    //3.5 Validate seniorId from route values
                    //check if seniorId was given in an endpoint
                    var seniorIdStr = context.Request.RouteValues["seniorId"]?.ToString();

                    if (seniorIdStr == null) //not all endpoints that need permission have to use seniorId (?)
                    {
                        await _next(context);
                        return;
                    }
                    //check if Id is an int
                    var isIdValidd = Int32.TryParse(seniorIdStr, out var seniorId);
                    if (!isIdValidd)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }

                    var seniorExists = profiles
                        .Where(p => p.SeniorId == seniorId)
                        .Any();

                    if (!seniorExists)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
        }

        await _next(context);
    }
}
