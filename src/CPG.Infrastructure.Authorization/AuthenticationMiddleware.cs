using CPG.Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Authorization;

public class AuthenticationMiddleware(IAuthenticationSchemeProvider schemes, RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly string userKycStatus="KycVerified";
    public IAuthenticationSchemeProvider Schemes { get; set; } = schemes ?? throw new ArgumentNullException(nameof(schemes));

    public async Task Invoke(HttpContext context, IMediator mediator)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                string? kycStatus = context.User.Claims.SingleOrDefault(c => c.Type == "status")?.Value;

                if (kycStatus != userKycStatus)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Data = string.Empty,
                        Message = string.Empty,
                        Action = "IdpProfileNotFound",
                        Errors = string.Empty,
                    });
                    return;
                }

                if (result?.Principal != null)
                    context.User = await ClonePrincipal(result.Principal, mediator);
            }
        }
        await _next(context);
    }


    private static async Task<ClaimsPrincipal> ClonePrincipal(ClaimsPrincipal principal, IMediator mediator)
    {
        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity)clone.Identity;

        var user = await mediator.Send(new GetUserAuthenticateQuery());
        if (user is null)
            return principal;

        newIdentity.AddClaim(new Claim(ClaimTypes.Name, user?.FirstName, ClaimValueTypes.String));
        newIdentity.AddClaim(new Claim(ClaimTypes.Surname, user?.LastName, ClaimValueTypes.String));
        newIdentity.AddClaim(new Claim(ClaimTypes.Sid, user?.IDPId, ClaimValueTypes.String));
        newIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, user?.PhoneNumber, ClaimValueTypes.String));
        newIdentity.AddClaim(new Claim(type: "NationalCode", value: user?.NationalCode));
        newIdentity.AddClaim(new Claim(type: "CompanyId", value: user?.CompanyId.ToString()));
        newIdentity.AddClaim(new Claim(type: "UserId", value: user?.Id.ToString()));
        newIdentity.AddClaim(new Claim(type: "ApplicationId", value: user?.ApplicationId.ToString()));
        newIdentity.AddClaim(new Claim(type: "ClientId", value: user?.IDPId.ToString()));

        return clone;
    }
}