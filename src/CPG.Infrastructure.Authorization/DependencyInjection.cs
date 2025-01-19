
using CPG.Application.Auth;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Authorization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            {
                services.AddTransient<ICurrentUser, CurrentUser>();
                services.AddHttpContextAccessor();
                services.AddTransient<IAuthService, JwtService>();
                services.AddScoped<IAuthenticationService, AuthenticationService>();
                var serviceProvider = services.BuildServiceProvider();

                var JwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfigViewModel>();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOption =>
                  {
                      //TODO: Remove RequireHttpsMetadata = false 
                      configureOption.RequireHttpsMetadata = false;
                      configureOption.Authority = JwtConfig.Authority;
                      configureOption.Audience = JwtConfig.ClientApiKey;
                      configureOption.MapInboundClaims = false;
                      configureOption.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidIssuer = JwtConfig.Authority,
                          ValidAudience = JwtConfig.ClientApiKey,
                          ValidateIssuer = JwtConfig.ValidateIssuer,
                          ValidateAudience = JwtConfig.ValidateAudience,
                          ValidateLifetime = JwtConfig.ValidateLifetime,
                          ClockSkew = TimeSpan.FromSeconds(Convert.ToInt32(JwtConfig.ClockSkew)),
                      };
                  });

                services.AddAuthorizationPolicies();
                return services;
            }
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

                options.AddPolicy(AuthPolicies.Roles.Admin, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, UserRoleType.SuperAdmin.GetValue());
                });

                options.AddPolicy(AuthPolicies.Roles.CompanyUser, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, UserRoleType.CompanyUser.GetValue());
                });

                options.AddPolicy(AuthPolicies.Roles.CustomerUser, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, UserRoleType.CustomerUser.GetValue());
                });

                options.AddPolicy(AuthPolicies.Roles.AdminOrCompanyUser, policy =>
                {
                    policy.RequireAssertion(context =>
                      context.User.HasClaim(c => c.Type == ClaimTypes.Role &&
                      c.Value == UserRoleType.SuperAdmin.GetValue() ||
                      c.Value == UserRoleType.CompanyUser.GetValue()));
                });
            });
        }
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder app)
            => app.UseAuthentication();

        public static IApplicationBuilder UseTokenAuthorization(this IApplicationBuilder app)
            => app.UseAuthorization();

        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
           => app.UseMiddleware<AuthenticationMiddleware>();

        public static IApplicationBuilder UseExternalServicesMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<ExternalServicesMiddleware>();
    }
}