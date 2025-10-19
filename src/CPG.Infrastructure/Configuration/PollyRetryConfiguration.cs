using CPG.Infrastructure.Policies;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace CPG.Infrastructure.Configuration;

/// <summary>
/// Polly retry policy configuration for HTTP clients.
/// Uses unified configuration from appsettings.json under Infrastructure:Polly:Retry (same config for HTTP and SOAP)
/// </summary>
public static class PollyRetryConfiguration
{
    /// <summary>
    /// Gets HTTP retry policy from unified configuration (same policy used for SOAP).
    /// </summary>
    /// <param name="config">Policy configuration from appsettings.json</param>
    /// <returns>Configured retry policy applied to all HTTP services</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy(PolicyConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        var settings = config.Retry;
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5XX and 408
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: settings.MaxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                {
                    // Calculate base delay (exponential or linear)
                    var delay = settings.UseExponentialBackoff
                        ? TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1) * settings.BaseDelaySeconds)
                        : TimeSpan.FromSeconds(retryAttempt * settings.BaseDelaySeconds);

                    // Add jitter to prevent thundering herd if enabled
                    if (settings.UseJitter)
                    {
                        delay += TimeSpan.FromMilliseconds(jitterer.Next(0, settings.MaxJitterMilliseconds));
                    }

                    return delay;
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt("HttpClient", outcome, timespan, retryCount, context);
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
