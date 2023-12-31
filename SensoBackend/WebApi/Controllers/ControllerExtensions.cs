﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SensoBackend.WebApi.Controllers;

public static class ControllerExtensions
{
    /// <summary> Retrieves accountId from claims </summary>
    /// <remarks>
    /// Assumes controller uses HasPermissionAttribute -
    /// checks for unauthorized users are not done and will result in error
    /// </remarks>
    public static int GetAccountId(this ControllerBase controller)
    {
        var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
        IEnumerable<Claim> claims = identity!.Claims;
        var accountIdStr = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        var accountId = int.Parse(accountIdStr);
        return accountId;
    }
}
