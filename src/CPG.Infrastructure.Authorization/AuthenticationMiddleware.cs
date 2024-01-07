using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.Queries;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IAuthenticationService = CPG.Domain.SharedKernel.IAuthenticationService;
namespace CPG.Infrastructure.Authorization
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(IAuthenticationSchemeProvider schemes, RequestDelegate next)
        {
            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public IAuthenticationSchemeProvider Schemes { get; set; }

        public async Task Invoke(HttpContext context, IMediator mediator)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                    if (result?.Principal != null)
                        context.User = await ClonePrincipal(result.Principal,mediator);
                }
            }
            await _next(context);
        }


        public async Task<ClaimsPrincipal> ClonePrincipal(ClaimsPrincipal principal,IMediator mediator)
        {
            var clone = principal.Clone();
            var newIdentity = (ClaimsIdentity)clone.Identity;
           
            var user = await mediator.Send(new GetUserAuthenticateQuery());
            if (user == null)
                return principal;
          
            newIdentity.AddClaim(new Claim(ClaimTypes.Name, user?.FirstName, ClaimValueTypes.String));
            newIdentity.AddClaim(new Claim(ClaimTypes.Surname, user?.LastName, ClaimValueTypes.String));
            newIdentity.AddClaim(new Claim(ClaimTypes.Sid, user?.IDPId, ClaimValueTypes.String));
            newIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, user?.PhoneNumber, ClaimValueTypes.String));
            newIdentity.AddClaim(new Claim(type: "NationalCode", value: user?.NationalCode));
            newIdentity.AddClaim(new Claim(type: "CompanyId", value: user?.CompanyId.ToString()));
            newIdentity.AddClaim(new Claim(type: "UserId", value: user?.Id.ToString()));
            newIdentity.AddClaim(new Claim(type: "AuditType", value: user?.AuditType.ToString()));
            newIdentity.AddClaim(new Claim(type: "ApplicationId", value: user?.ApplicationId.ToString()));
            newIdentity.AddClaim(new Claim(type: "ClientId", value: user?.IDPId.ToString()));

            return clone;
        }
    }
}