using Charisma.MessagingContracts.UsersManagement.User;
using CPG.Application.Shared;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Authorization;
using CPG.Infrastructure.ErrorHandling;
using CPG.Infrastructure.Masstransit.Consumer;
using CPG.Infrastructure.Masstransit.Consumer.UserRegistered;
using CPG.Infrastructure.Minio;
using CPG.Infrastructure.Persistence;
using CPG.Infrastructure.RabbitMQ;
using CPG.Infrastructure.Time;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.AspNetCore;

namespace CPG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddDatabase(configuration)
            .AddGraphQLQueries()
            .AddTokenAuthentication(configuration)
            .AddTransient<ICurrentDateTime, CurrentDateTime>()
            .AddMasstransitInfrastructure(configuration)
            .AddScoped<IMinioClient, MinioClient>()
            .AddScoped<IMinioProvider, MinioProvider>()
            .AddMinio(configuration)
            .AddHttpClient()
            .AddTransient<IHttpClientFactoryService, HttpClientFactoryService>();

    public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        _ = bool.TryParse(configuration["Minio:WithSSL"], out bool withSSL);

        _ = services.AddMinio(options =>
        {
            options.Endpoint = configuration["Minio:EndPoint"]!;
            options.AccessKey = configuration["Minio:AccessKey"]!;
            options.SecretKey = configuration["Minio:SecretKey"]!;
            options.ConfigureClient(client => _ = client.WithSSL(withSSL));
        });
        return services;
    }

    public static IServiceCollection AddMasstransitInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
       .AddMassTransit(x =>
       {
           var rabbitMqConfig = configuration.GetSection("Infrastructure:RabbitMQ").Get<RabbitMqConfig>();

           x.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter("pay__", false));
           x.AddConsumer<UserRegisteredConsumer, UserRegisteredConsumerDefinition>();
           x.AddConsumer<FaultConsumer>();

           x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConfig.Uri);

                cfg.Message<Fault>(f =>
            {
                f.SetEntityName("pay__fault");
            });

                cfg.Message<Fault<IUserRegistered>>(f =>
            {
                f.SetEntityName("pay__user_registered_fault");
            });

                cfg.ConfigureEndpoints(context);
            });
       });

        return services;
    }

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
            // .UseAuthenticationMiddleware()
            .UseGraphQLQueries(configuration.GetSection("Infrastructure:GraphQL"), env)
            .UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());
            });
}