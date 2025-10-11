using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace CPG.Infrastructure.Logging.Enrichers;

/// <summary>
/// Serilog enricher to add correlation_id to all log entries
/// Extracts from X-Correlation-ID header or generates from TraceIdentifier
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        // Try to get correlation ID from header first, fallback to TraceIdentifier
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].ToString();
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = httpContext.TraceIdentifier;
        }

        if (!string.IsNullOrEmpty(correlationId))
        {
            var property = propertyFactory.CreateProperty("correlation_id", correlationId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}

/// <summary>
/// Enricher factory for CorrelationIdEnricher
/// Used by Serilog configuration
/// </summary>
public static class CorrelationIdEnricherExtensions
{
    public static LoggerConfiguration WithCorrelationIdEnricher(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        IServiceProvider serviceProvider)
    {
        if (enrichmentConfiguration == null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        return enrichmentConfiguration.With(new CorrelationIdEnricher(httpContextAccessor));
    }
}
