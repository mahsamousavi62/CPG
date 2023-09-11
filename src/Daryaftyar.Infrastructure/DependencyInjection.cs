using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Infrastructure.Authorization;
using Daryaftyar.Infrastructure.ErrorHandling;
using Daryaftyar.Infrastructure.Persistence;
using Daryaftyar.Infrastructure.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daryaftyar.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddDatabase(configuration)
                .AddGraphQLQueries()
                .AddTokenAuthentication(configuration)
                .AddTransient<ICurrentDateTime, CurrentDateTime>();

        public static IApplicationBuilder UseInfrastructure(
            this IApplicationBuilder app,
            IConfiguration configuration,
            IWebHostEnvironment env)
            => app
                .UseHttpsRedirection()
                .UseRouting()
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseTokenAuthentication()
                .UseTokenAuthorization()
                .UseGraphQLQueries(configuration.GetSection("Infrastructure:GraphQL"), env);
    }
}