using Microsoft.AspNetCore.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Authorization;

public sealed class HasPermissionAttribute(Permission permission) : AuthorizeAttribute
{
    public Permission Permission { get; } = permission;
}
