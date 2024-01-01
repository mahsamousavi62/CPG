using CPG.Application.UseCases.Common.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;
using CPG.Infrastructure.Persistence.GraphQL.Queries;
using CPG.Infrastructure.Persistence.GraphQL.Types;
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
using System;
using System.Linq;

namespace CPG.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        private const string ConnectionStringConfigName = "CPGConnectionString";

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            
            services
                .AddDbContext<WriteDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging()
                        .UseSqlServer(configuration.GetConnectionString(ConnectionStringConfigName));
                })
                .AddDbContext<ReadDbContext>(options =>
                {
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
                .AddScoped<IRedisCaheService, RedisCacheService>();

            _ = bool.TryParse(configuration["Redis:Enable"], out var enableRedis);

            if (enableRedis)
            {
                var redisConfig = configuration.GetSection("Redis").Get<RedisConfig>();
                services.AddStackExchangeRedisCache(options => options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = { $"{redisConfig.Server}:{redisConfig.Port}" },
                    Password = redisConfig.Password,
                });
            }
            services.AddDistributedMemoryCache();
            return services;
        }

        public static IServiceCollection AddGraphQLQueries(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<GetApplicationSettingsQuery>()
                .AddQueryType<CompanyReadModelQueries>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddType<CompanyReadModelType>();

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
                    var logger = services.GetRequiredService<ILogger<WriteDbContext>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            return webHost;
        }
    }
}