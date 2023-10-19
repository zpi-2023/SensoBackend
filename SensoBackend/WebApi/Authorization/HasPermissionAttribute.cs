using Microsoft.AspNetCore.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Authorization;

internal sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public Permission Permission { get; set; }

    public HasPermissionAttribute(Permission permission)
    {
        Permission = permission;
    }
}
