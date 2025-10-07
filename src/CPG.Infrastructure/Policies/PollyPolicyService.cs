using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// Service for managing Polly resilience policies (Retry, Circuit Breaker, Timeout)
/// </summary>
public interface IPollyPolicyService
{
    /// <summary>
    /// Get combined HTTP policy (Retry + Circuit Breaker + Timeout)
    /// </summary>
    IAsyncPolicy<HttpResponseMessage> GetHttpPolicy(string policyName = "default");

    /// <summary>
    /// Get retry policy for SOAP services with exception handling
    /// </summary>
    IAsyncPolicy GetSoapRetryPolicy(string serviceName = "soap");

    /// <summary>
    /// Execute action with retry policy for SOAP services
    /// </summary>
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, string serviceName = "soap");

    /// <summary>
    /// Execute action with retry and timeout for SOAP services
    /// </summary>
    Task<T> ExecuteWithPolicyAsync<T>(Func<Task<T>> action, string serviceName = "soap");
}

public class PollyPolicyService : IPollyPolicyService
{
    private readonly PolicyConfig _config;
    private readonly ILogger<PollyPolicyService> _logger;

    public PollyPolicyService(IOptions<PolicyConfig> config, ILogger<PollyPolicyService> logger)
    {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get combined HTTP policy (Retry + Circuit Breaker + Timeout)
    /// </summary>
    public IAsyncPolicy<HttpResponseMessage> GetHttpPolicy(string policyName = "default")
    {
        var retryPolicy = GetHttpRetryPolicy(policyName);
        var circuitBreakerPolicy = GetHttpCircuitBreakerPolicy(policyName);
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(_config.Timeout.TimeoutSeconds),
            TimeoutStrategy.Pessimistic,
            (context, timespan, task) =>
            {
                _logger.LogWarning("HTTP request timeout after {TimeoutSeconds}s for policy {PolicyName}",
                    _config.Timeout.TimeoutSeconds, policyName);
                return Task.CompletedTask;
            });

        // Order: Timeout -> Retry -> Circuit Breaker
        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    /// <summary>
    /// Get retry policy for HTTP requests
    /// </summary>
    private IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy(string policyName)
    {
        var retryConfig = _config.Retry;

        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5xx and 408
            .Or<TimeoutRejectedException>() // Timeout exceptions
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // 429
            .WaitAndRetryAsync(
                retryCount: retryConfig.MaxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(retryConfig.BaseDelaySeconds, retryAttempt));

                    // Add jitter to prevent thundering herd
                    if (retryConfig.UseJitter)
                    {
                        var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
                        delay = delay.Add(jitter);
                    }

                    return delay;
                },
                onRetry: async (outcome, timespan, retryCount, context) =>
                {
                    var responseBody = string.Empty;
                    var statusCode = "Unknown";

                    if (outcome.Result != null)
                    {
                        statusCode = outcome.Result.StatusCode.ToString();
                        try
                        {
                            // Log complete response body on timeout or error
                            responseBody = await outcome.Result.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(responseBody) && responseBody.Length > 5000)
                            {
                                responseBody = responseBody.Substring(0, 5000) + "... [truncated]";
                            }
                        }
                        catch
                        {
                            responseBody = "[Could not read response body]";
                        }
                    }

                    _logger.LogWarning(
                        outcome.Exception,
                        "HTTP retry attempt {RetryCount} of {MaxRetryAttempts} for {PolicyName}. " +
                        "Waiting {DelayMs}ms before next retry. StatusCode: {StatusCode}. " +
                        "Response: {ResponseBody}. Exception: {ExceptionMessage}",
                        retryCount,
                        retryConfig.MaxRetryAttempts,
                        policyName,
                        timespan.TotalMilliseconds,
                        statusCode,
                        responseBody,
                        outcome.Exception?.Message ?? "None");
                });
    }

    /// <summary>
    /// Get circuit breaker policy for HTTP requests
    /// </summary>
    private IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(string policyName)
    {
        if (!_config.CircuitBreaker.Enabled)
        {
            return Policy.NoOpAsync<HttpResponseMessage>();
        }

        var cbConfig = _config.CircuitBreaker;

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: (double)cbConfig.FailureThreshold / 10, // Convert to 0-1 range
                samplingDuration: TimeSpan.FromSeconds(cbConfig.SamplingDurationSeconds),
                minimumThroughput: cbConfig.MinimumThroughput,
                durationOfBreak: TimeSpan.FromSeconds(cbConfig.DurationOfBreakSeconds),
                onBreak: (result, duration) =>
                {
                    _logger.LogError(
                        "Circuit breaker opened for {PolicyName}. Breaking for {DurationSeconds}s. Reason: {Reason}",
                        policyName,
                        duration.TotalSeconds,
                        result.Exception?.Message ?? result.Result?.StatusCode.ToString() ?? "Unknown");
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset for {PolicyName}", policyName);
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker half-open for {PolicyName}, testing if service recovered", policyName);
                });
    }

    /// <summary>
    /// Get retry policy for SOAP services
    /// </summary>
    public IAsyncPolicy GetSoapRetryPolicy(string serviceName = "soap")
    {
        var retryConfig = _config.Retry;

        return Policy
            .Handle<EndpointNotFoundException>() // SOAP service not available
            .Or<CommunicationException>() // SOAP communication errors
            .Or<TimeoutException>() // Timeout
            .Or<ServerTooBusyException>() // Server busy
            .Or<Exception>(ex => ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                                ex.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase))
            .WaitAndRetryAsync(
                retryCount: retryConfig.MaxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(retryConfig.BaseDelaySeconds, retryAttempt));

                    // Add jitter
                    if (retryConfig.UseJitter)
                    {
                        var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
                        delay = delay.Add(jitter);
                    }

                    return delay;
                },
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "SOAP retry attempt {RetryCount} of {MaxRetryAttempts} for {ServiceName}. " +
                        "Waiting {DelayMs}ms before next retry",
                        retryCount,
                        retryConfig.MaxRetryAttempts,
                        serviceName,
                        timespan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Execute action with retry policy for SOAP services
    /// </summary>
    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, string serviceName = "soap")
    {
        var retryPolicy = GetSoapRetryPolicy(serviceName);
        return await retryPolicy.ExecuteAsync(action);
    }

    /// <summary>
    /// Execute action with retry and timeout for SOAP services
    /// </summary>
    public async Task<T> ExecuteWithPolicyAsync<T>(Func<Task<T>> action, string serviceName = "soap")
    {
        var retryPolicy = GetSoapRetryPolicy(serviceName);

        var timeoutPolicy = Policy.TimeoutAsync(
            TimeSpan.FromSeconds(_config.Timeout.SoapTimeoutSeconds),
            TimeoutStrategy.Pessimistic,
            (context, timespan, task) =>
            {
                _logger.LogWarning(
                    "SOAP service timeout after {TimeoutSeconds}s for {ServiceName}",
                    _config.Timeout.SoapTimeoutSeconds,
                    serviceName);
                return Task.CompletedTask;
            });

        // Wrap retry with timeout
        var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

        return await combinedPolicy.ExecuteAsync(action);
    }
}
