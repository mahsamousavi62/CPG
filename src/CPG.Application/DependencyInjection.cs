using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CPG.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var infrastructureAssembly = AppDomain.CurrentDomain.Load("CPG.Infrastructure.Persistence");

            var allAssemblies = applicationAssemblies.Append(infrastructureAssembly).ToArray();

            return services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(allAssemblies));
        }
    }
}
