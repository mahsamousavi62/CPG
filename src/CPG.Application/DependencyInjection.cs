using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
