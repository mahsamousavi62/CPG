using Charisma.MessagingContracts.UsersManagement.User;
using CPG.Application.Shared;
using CPG.Application.UseCases.Common.Queries;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Authorization;
using CPG.Infrastructure.ErrorHandling;
using CPG.Infrastructure.Masstransit.Consumer;
using CPG.Infrastructure.Masstransit.Consumer.UserRegistered;
using CPG.Infrastructure.Persistence;
using CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;
using CPG.Infrastructure.Persistence.GraphQL.Queries;
using CPG.Infrastructure.Persistence.GraphQL.Types;
using CPG.Infrastructure.RabbitMQ;
using CPG.Infrastructure.Time;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using Minio;
using Minio.AspNetCore;
using Minio.Credentials;

namespace CPG.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddDatabase(configuration)
                .AddGraphQLQueries()
                .AddTokenAuthentication(configuration)
                .AddTransient<ICurrentDateTime, CurrentDateTime>()
                .AddMasstransitInfrastructure(configuration)
                .AddHttpClient()
                .AddMinio(configuration)
                .AddTransient<IHttpClientFactoryService, HttpClientFactoryService>()
                .AddTransient<IMinioClient, MinioClient>();

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
}