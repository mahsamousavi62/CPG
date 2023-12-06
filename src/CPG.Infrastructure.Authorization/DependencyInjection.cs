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
                var repository = serviceProvider.GetRequiredService<IApplicationSettingsRepository>();
                var applicationConfigViewModel = repository.GetAllApplicationSettings().GetAwaiter().GetResult();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOption =>
                  {
                      configureOption.Authority = applicationConfigViewModel.Authority;
                      configureOption.Audience = applicationConfigViewModel.ClientApiKey;
                      configureOption.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidIssuer = applicationConfigViewModel.Authority,
                          ValidAudience = applicationConfigViewModel.ClientApiKey,
                          ValidateIssuer = applicationConfigViewModel.ValidateIssuer,
                          ValidateAudience = applicationConfigViewModel.ValidateAudience,
                          ValidateLifetime = applicationConfigViewModel.ValidateLifetime,
                          ClockSkew = TimeSpan.FromSeconds(Convert.ToInt32(applicationConfigViewModel.ClockSkew)),
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