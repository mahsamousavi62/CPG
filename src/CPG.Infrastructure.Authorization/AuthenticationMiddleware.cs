using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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

        public async Task Invoke(HttpContext context, IAuthenticationService authenticationService)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await context.AuthenticateAsync(defaultAuthenticate.Name);

                var clientId = await authenticationService.GetClientId();

                    if (result?.Principal != null)
                    {
                        var claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Name, $"mahsa", ClaimValueTypes.String));

                        claims.Add(new Claim(ClaimTypes.Surname, "mousavi", ClaimValueTypes.String));

                        claims.Add(new Claim(ClaimTypes.Sid, "123", ClaimValueTypes.String));

                        claims.Add(new Claim(ClaimTypes.MobilePhone, "091222222", ClaimValueTypes.String));

                        claims.Add(new Claim(type: "companyId", value: "1"));

                        //create principal for the current authentication scheme
                        var userIdentity = new ClaimsIdentity(claims, "Authentication");
                        var userPrincipal = new ClaimsPrincipal(userIdentity);
                        
                                              context.User = userPrincipal;
                        

                    }
                }
            }

            await _next(context);
        }
    }
}