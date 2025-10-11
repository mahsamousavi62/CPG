using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq;

namespace CPG.Infrastructure.Logging.Enrichers;

/// <summary>
/// Serilog enricher to add user context to all log entries
/// Extracts user_id, company_id, application_id from JWT claims
/// </summary>
public class UserContextEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Claims == null || !httpContext.User.Claims.Any())
            return;

        // Extract user_id from claims
        var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var userId))
        {
            var userIdProperty = propertyFactory.CreateProperty("user_id", userId);
            logEvent.AddPropertyIfAbsent(userIdProperty);
        }

        // Extract company_id from claims
        var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
        if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var companyId))
        {
            var companyIdProperty = propertyFactory.CreateProperty("company_id", companyId);
            logEvent.AddPropertyIfAbsent(companyIdProperty);
        }

        // Extract application_id from claims
        var applicationIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
        if (!string.IsNullOrEmpty(applicationIdClaim) && long.TryParse(applicationIdClaim, out var applicationId))
        {
            var applicationIdProperty = propertyFactory.CreateProperty("application_id", applicationId);
            logEvent.AddPropertyIfAbsent(applicationIdProperty);
        }

        // Extract client_id from claims
        var clientIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
        if (!string.IsNullOrEmpty(clientIdClaim))
        {
            var clientIdProperty = propertyFactory.CreateProperty("client_id", clientIdClaim);
            logEvent.AddPropertyIfAbsent(clientIdProperty);
        }

        // Extract mobile_phone from claims
        var mobilePhoneClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "MobilePhone")?.Value;
        if (!string.IsNullOrEmpty(mobilePhoneClaim))
        {
            var mobilePhoneProperty = propertyFactory.CreateProperty("mobile_phone", mobilePhoneClaim);
            logEvent.AddPropertyIfAbsent(mobilePhoneProperty);
        }
    }
}

/// <summary>
/// Enricher factory for UserContextEnricher
/// Used by Serilog configuration
/// </summary>
public static class UserContextEnricherExtensions
{
    public static LoggerConfiguration WithUserContextEnricher(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        IServiceProvider serviceProvider)
    {
        if (enrichmentConfiguration == null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        return enrichmentConfiguration.With(new UserContextEnricher(httpContextAccessor));
    }
}
