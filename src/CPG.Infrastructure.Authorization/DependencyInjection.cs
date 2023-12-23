using CPG.Application.Auth;
using CPG.Application.UseCases.Common.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace CPG.Infrastructure.Authorization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            {
                services.AddTransient<ICurrentUser, CurrentUser>();
                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddTransient<IAuthService, JwtService>();
                services.AddScoped<IAuthenticationService, AuthenticationService>();
                var serviceProvider = services.BuildServiceProvider();

                var JwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfigViewModel>();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOption =>
                  {
                      configureOption.Authority = JwtConfig.Authority;
                      configureOption.Audience = JwtConfig.ClientApiKey;
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

                return services;
            }
        }
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder app)
            => app.UseAuthentication();

        public static IApplicationBuilder UseTokenAuthorization(this IApplicationBuilder app)
            => app.UseAuthorization();

        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
           => app.UseMiddleware<AuthenticationMiddleware>();
    }

}