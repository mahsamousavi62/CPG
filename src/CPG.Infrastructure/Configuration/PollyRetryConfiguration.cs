using CPG.Infrastructure.Policies;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace CPG.Infrastructure.Configuration;

/// <summary>
/// Polly retry policy configuration for HTTP clients with exponential backoff and jitter.
/// All policies are now configuration-driven from appsettings.json under Infrastructure:Polly:Retry:Http
/// </summary>
public static class PollyRetryConfiguration
{
    /// <summary>
    /// Gets HTTP retry policy from configuration for a specific service or uses default.
    /// </summary>
    /// <param name="config">Policy configuration from appsettings.json</param>
    /// <param name="serviceName">Optional service name (CharisPay, IdpClient, NeoBank, etc.). If null, uses Default settings.</param>
    /// <returns>Configured retry policy for the specified service</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy(PolicyConfig config, string serviceName = null)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        // Get service-specific settings or fall back to default
        var settings = serviceName != null && config.Retry.Http.Services.ContainsKey(serviceName)
            ? config.Retry.Http.Services[serviceName]
            : config.Retry.Http.Default;

        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5XX and 408
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                sleepDurationProvider: retryAttempt =>
                {
                    // Calculate base delay (exponential or linear)
                    var delay = settings.UseExponentialBackoff
                        ? TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1) * settings.BaseDelaySeconds)
                        : TimeSpan.FromSeconds(retryAttempt * settings.BaseDelaySeconds);

                    // Add jitter to prevent thundering herd
                    return delay + TimeSpan.FromMilliseconds(jitterer.Next(0, settings.MaxJitterMilliseconds));
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt(serviceName ?? "Default", outcome, timespan, retryCount, context);
                });
    }

    /// <summary>
    /// Logs retry attempts with details about the failed request.
    /// Integrates with existing logging infrastructure to track retry behavior.
    /// </summary>
    private static void LogRetryAttempt(
        string clientName,
        DelegateResult<HttpResponseMessage> outcome,
        TimeSpan timespan,
        int retryCount,
        Context context)
    {
        var requestUri = outcome.Result?.RequestMessage?.RequestUri?.ToString() ?? "Unknown";
        var statusCode = outcome.Result?.StatusCode.ToString() ?? "N/A";
        var exceptionMessage = outcome.Exception?.Message ?? "No exception";

        // Log retry attempt to console for immediate visibility
        // Note: ILogService cannot be easily injected into Polly policies without a service provider context
        // The existing HttpProvider logging will capture the final result with all retry metadata
        Console.WriteLine($"[Polly Retry] Client: {clientName}, Attempt: {retryCount}, " +
            $"URI: {requestUri}, Status: {statusCode}, Exception: {exceptionMessage}, " +
            $"Waiting: {timespan.TotalSeconds:F1}s");

        // Store retry information in Polly context for correlation with CallLogModel
        context["RetryCount"] = retryCount;
        context["ClientName"] = clientName;
        context["LastRetryDelay"] = timespan.TotalSeconds;

        // Store the correlation ID if available (from HttpContext)
        if (context.TryGetValue("CorrelationId", out var correlationId))
        {
            context["CorrelationId"] = correlationId;
        }
    }

}
