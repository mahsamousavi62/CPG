using CPG.Application.Shared.Behaviours;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CPG.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var infrastructureAssembly = AppDomain.CurrentDomain.Load("CPG.Infrastructure.Persistence");

        var allAssemblies = applicationAssemblies.Append(infrastructureAssembly).ToArray();

        return services                
            .AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(allAssemblies);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            });
    }
}
