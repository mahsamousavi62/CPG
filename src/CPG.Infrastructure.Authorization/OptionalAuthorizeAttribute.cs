using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Authorization;

/// <summary>
/// Authorization attribute that validates token if present, but allows anonymous access if no token is provided
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class OptionalAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return;
        }

        var authorizationService = context.HttpContext.RequestServices
            .GetService(typeof(IAuthorizationService)) as IAuthorizationService;

        if (authorizationService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

    }
}