using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CPG.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var infrastructureAssembly = AppDomain.CurrentDomain.Load("CPG.Infrastructure.Persistence");

            return services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(infrastructureAssembly));
        }
    }
}
