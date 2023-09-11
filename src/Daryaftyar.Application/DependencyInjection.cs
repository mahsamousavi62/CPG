using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daryaftyar.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var infrastructureAssembly = AppDomain.CurrentDomain.Load("Daryaftyar.Infrastructure.Persistence");
            
            return services
                .AddMediatR(Assembly.GetExecutingAssembly(), infrastructureAssembly);
        }
    }
}
