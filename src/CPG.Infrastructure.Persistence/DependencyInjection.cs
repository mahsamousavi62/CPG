using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;
using CPG.Infrastructure.Persistence.GraphQL.Queries;
using CPG.Infrastructure.Persistence.Interceptors;
using CPG.Infrastructure.Persistence.Redis;
using CPG.Infrastructure.Persistence.Repositories;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        private const string ConnectionStringConfigName = "CPGConnectionString";

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            services.AddScoped<IInterceptor, DatabaseLoggingInterceptor>();

            services
                .AddDbContext<WriteDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.AddInterceptors(sp.GetServices<IInterceptor>().OfType<DatabaseLoggingInterceptor>());

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging()
                        .UseSqlServer(configuration.GetConnectionString(ConnectionStringConfigName));
                })
                .AddDbContext<ReadDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<IInterceptor>().OfType<DatabaseLoggingInterceptor>());

                    options.EnableDetailedErrors();
                    options.UseSqlServer(configuration.GetConnectionString(ConnectionStringConfigName))
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Information);
                })
                .AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>))
                .AddScoped(typeof(IAggregateReadRepository<>), typeof(AggregateRepository<>))
                .AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>))
                .AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>))
                .AddScoped(typeof(ICommonServiceRepository<>), typeof(CommonServiceRepository<>))
                .AddScoped(typeof(IApplicationSettingsRepository), typeof(ApplicationSettingsRepository))
                .AddScoped<IRedisCacheService, RedisCacheService>();

            _ = bool.TryParse(configuration["Redis:Enable"], out var enableRedis);

            if (enableRedis)
            {
                var redisConfig = configuration.GetSection("Redis").Get<RedisConfiguration>();
                services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfig.ConfigurationOptions));
                services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(redisConfig);
            }
            services.AddDistributedMemoryCache();
            return services;
        }

        public static IServiceCollection AddGraphQLQueries(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<ReadModelQueries>()
                .AddProjections()
                .AddFiltering()
                .AddSorting();

            services.AddErrorFilter<GraphQLErrorFilter>();

            return services;
        }

        public static IApplicationBuilder UseGraphQLQueries(
            this IApplicationBuilder app,
            IConfiguration graphQlConfiguration,
            IWebHostEnvironment env)
        {
            var graphQLEndpoint = graphQlConfiguration.GetSection("EndpointUrl").Value;

            return app.UseEndpoints(x => x.MapGraphQL(graphQLEndpoint)
                .WithOptions(new GraphQLServerOptions
                {
                    Tool =
                    {
                        Enable = env.IsDevelopment()
                    }
                }));
        }

        public static IHost MigrateDatabase(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<WriteDbContext>();
                    if (db.Database.GetPendingMigrations().Any())
                    {
                        db.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var logService = services.GetRequiredService<CPG.Domain.SharedKernel.Logging.ILogService>();
                    var callLog = CallLogModel.CreateError(
                        serviceName: "Database",
                        providerName: "MigrateDatabase",
                        requestUri: "Database Migration",
                        requestBody: null,
                        responseBody: ex.Message,
                        exception: ex,
                        auditType: Enums.AuditType.Client
                    );
                    logService.LogError(callLog);
                }
            }

            return webHost;
        }
    }
}