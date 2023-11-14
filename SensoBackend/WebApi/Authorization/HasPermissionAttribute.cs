using Microsoft.AspNetCore.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public Permission Permission { get; }

    public HasPermissionAttribute(Permission permission) => Permission = permission;
}
