using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CPG.Infrastructure.Authorization;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public long UserId => GetUserId();

    private long GetUserId()
    {
       // var subid = await _authenticationService.GetDataFromClaim<string>("sub");
      
        //TODO: select userid from user where idpuserid=sub
        var claims = _httpContextAccessor.HttpContext?.User.Claims
                     ?? throw new ArgumentException("Cannot obtain UserId value from JWT token.");

        var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value?? "1";

        return long.Parse(userId);
    }
}
