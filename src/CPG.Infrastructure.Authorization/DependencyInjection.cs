using CPG.Application.Auth;
using CPG.Domain.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CPG.Infrastructure.Authorization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            {
                // var serviceProvider = services.BuildServiceProvider();

                //var mediator = serviceProvider.GetRequiredService<IMediator>();
                //var authenticationConfig = ( mediator.Send(new GetAuthenticationAppSettingQuery())).GetAwaiter().GetResult();

                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOption =>
                //  {

                //      configureOption.Authority = authenticationConfig.Authority;
                //      configureOption.Audience = authenticationConfig.ClientApiKey;

                //      configureOption.TokenValidationParameters = new TokenValidationParameters
                //      {
                //          ValidIssuer = authenticationConfig.Authority,
                //          ValidAudience = authenticationConfig.ClientApiKey,
                //          ValidateIssuer = authenticationConfig.ValidateIssuer,
                //          ValidateAudience = authenticationConfig.ValidateAudience,
                //          ValidateLifetime = authenticationConfig.ValidateLifetime,
                //          ClockSkew = TimeSpan.FromSeconds(Convert.ToInt32(authenticationConfig.ClockSkew)),
                //      };
                //  });

                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddTransient<IAuthService, JwtService>();
                services.AddTransient<ICurrentUser, CurrentUser>();

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