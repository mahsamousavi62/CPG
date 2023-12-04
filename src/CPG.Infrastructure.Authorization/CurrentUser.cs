using System;
using System.Linq;
using System.Security.Claims;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace CPG.Infrastructure.Authorization;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public long UserId => GetUserId();

    private long GetUserId()
    {

        return 1;
        var claims = _httpContextAccessor.HttpContext?.User.Claims 
                     ?? throw new ArgumentException("Cannot obtain UserId value from JWT token.");

        var userId = claims.SingleOrDefault(x => x.Type == ClaimTypes.Sid)?.Value 
               ?? throw new ArgumentException("Cannot obtain UserId value from JWT token.");

        return long.Parse(userId);
    }
}
