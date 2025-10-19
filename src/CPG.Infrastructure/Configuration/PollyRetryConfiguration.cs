using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace CPG.Infrastructure.Configuration;

/// <summary>
/// Polly retry policy configuration for HTTP clients with exponential backoff and jitter.
/// Provides factory methods for creating retry policies for different service types.
/// </summary>
public static class PollyRetryConfiguration
{
    /// <summary>
    /// Gets the default retry policy for unnamed HTTP clients.
    /// Applies 3 retries with exponential backoff (1s, 2s, 4s) and up to 1s jitter.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
    {
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5XX and 408
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)) // 1s, 2s, 4s
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Logging will be handled by LogRetryAttempt if ILogService is available
                    LogRetryAttempt("DefaultClient", outcome, timespan, retryCount, context);
                });
    }

    /// <summary>
    /// Gets retry policy for payment providers (CharisPay, AsanPardakht, CharismaCard).
    /// Applies 2 retries with linear backoff (1s, 2s) and up to 500ms jitter.
    /// Payment operations require fewer retries to avoid duplicate transactions.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetPaymentProviderRetryPolicy()
    {
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt) // 1s, 2s
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 500)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt("PaymentProvider", outcome, timespan, retryCount, context);
                });
    }

    /// <summary>
    /// Gets retry policy for identity provider (IDP).
    /// Applies 3 retries with exponential backoff (1s, 2s, 4s) and up to 1s jitter.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetIdentityProviderRetryPolicy()
    {
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)) // 1s, 2s, 4s
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt("IdentityProvider", outcome, timespan, retryCount, context);
                });
    }

    /// <summary>
    /// Gets retry policy for financial services (NeoBank).
    /// Applies 3 retries with exponential backoff (2s, 4s, 8s) and up to 2s jitter.
    /// Financial operations need longer delays for external systems to stabilize.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetFinancialServiceRetryPolicy()
    {
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // 2s, 4s, 8s
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 2000)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt("FinancialService", outcome, timespan, retryCount, context);
                });
    }

    /// <summary>
    /// Gets retry policy for CharismaCard services.
    /// Applies 2 retries with linear backoff (1s, 2s) and up to 500ms jitter.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCharismaCardRetryPolicy()
    {
        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt) // 1s, 2s
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 500)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt("CharismaCard", outcome, timespan, retryCount, context);
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

    /// <summary>
    /// Extension method to add retry policy configuration from appsettings.json.
    /// This will be used in Task 5 when configuration is added.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyFromConfiguration(
        IConfiguration configuration,
        string serviceName)
    {
        var retryCount = configuration.GetValue<int>($"Infrastructure:RetryPolicy:ServiceSpecific:{serviceName}:RetryCount", 3);
        var baseDelaySeconds = configuration.GetValue<int>($"Infrastructure:RetryPolicy:ServiceSpecific:{serviceName}:BaseDelaySeconds", 1);
        var maxJitterMs = configuration.GetValue<int>($"Infrastructure:RetryPolicy:ServiceSpecific:{serviceName}:MaxJitterMilliseconds", 1000);
        var useExponential = configuration.GetValue<bool>($"Infrastructure:RetryPolicy:Default:UseExponentialBackoff", true);

        var jitterer = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode && (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt =>
                {
                    var delay = useExponential
                        ? TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1) * baseDelaySeconds)
                        : TimeSpan.FromSeconds(retryAttempt * baseDelaySeconds);
                    return delay + TimeSpan.FromMilliseconds(jitterer.Next(0, maxJitterMs));
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogRetryAttempt(serviceName, outcome, timespan, retryCount, context);
                });
    }
}
