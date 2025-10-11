using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace CPG.Infrastructure.Logging.Enrichers;

/// <summary>
/// Serilog enricher to add request_id to all log entries
/// Extracts from X-Request-ID header or generates new GUID
/// </summary>
public class RequestIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        // Try to get request ID from header first
        var requestId = httpContext.Request.Headers["X-Request-ID"].ToString();

        // If not in header, check if already set in HttpContext.Items
        if (string.IsNullOrEmpty(requestId) && httpContext.Items.ContainsKey("RequestId"))
        {
            requestId = httpContext.Items["RequestId"]?.ToString();
        }

        // Generate new if still not available
        if (string.IsNullOrEmpty(requestId))
        {
            requestId = Guid.NewGuid().ToString("N").Substring(0, 12);
            httpContext.Items["RequestId"] = requestId;
        }

        var property = propertyFactory.CreateProperty("request_id", requestId);
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// Enricher factory for RequestIdEnricher
/// Used by Serilog configuration
/// </summary>
public static class RequestIdEnricherExtensions
{
    public static LoggerConfiguration WithRequestIdEnricher(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        IServiceProvider serviceProvider)
    {
        if (enrichmentConfiguration == null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        return enrichmentConfiguration.With(new RequestIdEnricher(httpContextAccessor));
    }
}
