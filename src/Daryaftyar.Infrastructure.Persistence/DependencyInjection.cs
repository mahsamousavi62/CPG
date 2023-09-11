using System;
using HotChocolate.AspNetCore;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Infrastructure.Persistence.DbContexts;
using Daryaftyar.Infrastructure.Persistence.GraphQL.ErrorHandling;
using Daryaftyar.Infrastructure.Persistence.GraphQL.Queries;
using Daryaftyar.Infrastructure.Persistence.GraphQL.Types;
using Daryaftyar.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Daryaftyar.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        private const string ConnectionStringConfigName = "DaryaftyarConnectionString";

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddDbContext<WriteDbContext>(options =>
                {
                    options.EnableDetailedErrors();
                    options.UseSqlServer(configuration.GetConnectionString(ConnectionStringConfigName));
                })
                .AddDbContext<ReadDbContext>(options =>
                {
                    options.EnableDetailedErrors();
                    options.UseSqlServer(configuration.GetConnectionString(ConnectionStringConfigName));
                })
                .AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>))
                .AddScoped(typeof(IAggregateReadRepository<>), typeof(AggregateRepository<>));

        public static IServiceCollection AddGraphQLQueries(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<BookReadModelQueries>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddType<BookReadModelType>();

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
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Apply logger here
                    //var logger = services.GetRequiredService<ILogger<Program>>();
                    //logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            return webHost;
        }
    }
}