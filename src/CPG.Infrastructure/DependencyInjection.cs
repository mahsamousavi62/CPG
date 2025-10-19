using Charisma.MessagingContracts.UsersManagement.User;
using CPG.Application.Shared;
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
using System.Net.Http;
using System;
using Polly;
using System.Net;
using Confluent.Kafka;
using System.Net.Http.Headers;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication.Charispay;
using CPG.Domain.SharedKernel.Communication.Idp;
using CPG.Infrastructure.Providers.Idp;
using CPG.Infrastructure.Providers.Charispay;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Infrastructure.Providers.Ipg;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication;
using CPG.Infrastructure.Providers;
using CPG.Application.Auth;
using CPG.Infrastructure.Logging;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Infrastructure.Providers.DirectDebit;
using CPG.Infrastructure.Providers.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Infrastructure.Cache;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Infrastructure.Providers.CharismaCard;
using CPG.Infrastructure.Configuration;
using CPG.Infrastructure.Policies;

namespace CPG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<ICharisPayProvider, CharisPayProvider>()
            .AddScoped<INeoBankService, NeoBankProvider>()
            .AddScoped<ICharismaCardService, CharismaCardProvider>()
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<IIdpProvider, IdpProvider>()
            .AddScoped<IIpgFactory, IpgFactory>()
            .AddScoped<IDirectDebitFactory, DirectDebitFactory>()
            .AddScoped<IIpgProvider, AsanPardakhtProvider>()
            .AddScoped<IDirectDebitProvider, VandarProvider>()
            .AddSingleton<ILogService, LogService>()
            .AddTransient<ICurrentDateTime, CurrentDateTime>()
            .AddTransient<IHttpProvider, HttpProvider>()
            .AddDatabase(configuration)
            .AddGraphQLQueries()
            .AddTokenAuthentication(configuration)
            //.AddMasstransitInfrastructure(configuration)
            .AddScoped<IMinioProvider, MinioProvider>()
            .AddMinio(configuration)
            .AddPollyPolicies(configuration)
            .AddHttpClientWithRetryPolicies(configuration)
            .AddConfigureHttpClientService(configuration);

    public static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Polly policy configuration
        services.Configure<PolicyConfig>(configuration.GetSection(PolicyConfig.SectionName));

        // Register Polly policy service
        services.AddSingleton<IPollyPolicyService, PollyPolicyService>();

        return services;
    }

    public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationConfigViewModel = configuration.GetSection("Infrastructure:Minio").Get<MinioConfigViewModel>(); ;

        services.AddMinio(configureClient => configureClient
          .WithEndpoint(applicationConfigViewModel.EndPoint)
          .WithCredentials(applicationConfigViewModel.AccessKey, applicationConfigViewModel.SecretKey)
          .WithSSL(applicationConfigViewModel.WithSSL));

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

    public static IServiceCollection AddHttpClientWithRetryPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        // Get Polly configuration
        var policyConfig = configuration.GetSection(PolicyConfig.SectionName).Get<PolicyConfig>() ?? new PolicyConfig();

        // Add HttpClient with unified retry policy for all HTTP clients
        services.AddHttpClient()
            .ConfigureHttpClientDefaults(builder =>
            {
                // Apply unified retry policy to all HTTP clients (named and unnamed)
                builder.AddPolicyHandler(PollyRetryConfiguration.GetHttpRetryPolicy(policyConfig));
            });

        return services;
    }

    public static IServiceCollection AddConfigureHttpClientService(this IServiceCollection services, IConfiguration configuration)
    {
        // Get Polly configuration
        var policyConfig = configuration.GetSection(PolicyConfig.SectionName).Get<PolicyConfig>() ?? new PolicyConfig();

        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthService>();
        var jwtConfig = authService.GetJwtConfig();
        var charisPayConfig = configuration.GetSection("Infrastructure:CharisPay").Get<CharisPayConfig>();
        var neoBankConfig = configuration.GetSection("Infrastructure:NeoBank").Get<NeoBankConfig>();
        var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();

        // All named HTTP clients use the same unified retry policy
        var retryPolicy = PollyRetryConfiguration.GetHttpRetryPolicy(policyConfig);

        services.AddHttpClient("charisPayClient", c =>
        {
            c.BaseAddress = new Uri(charisPayConfig.BaseUrl);
            c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(retryPolicy);

        services.AddHttpClient("idpClient", c =>
        {
            c.BaseAddress = new Uri(jwtConfig.Authority);
            c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(retryPolicy);

        services.AddHttpClient("neoBankClient", c =>
        {
            c.BaseAddress = new Uri(neoBankConfig.BaseUrl);
            c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(retryPolicy);

        services.AddHttpClient("asanpardakhtClient", c =>
        {
            c.BaseAddress = new Uri("https://ipgrest.asanpardakht.ir/");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        })
        .AddPolicyHandler(retryPolicy);

        services.AddHttpClient("charismaCardClient", c =>
        {
            c.BaseAddress = new Uri(charismaCardConfig.BaseUrl);
            c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(retryPolicy);

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IWebHostEnvironment env)
        => app
            .UseHttpsRedirection()
            .UseRouting()
            .UseTokenAuthentication()
            .UseAuthenticationMiddleware()
            .UseExternalServicesMiddleware()
            .UseTokenAuthorization()
            .UseMiddleware<ErrorHandlingMiddleware>()
            .UseMiddleware<LoggingMiddleware>()
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