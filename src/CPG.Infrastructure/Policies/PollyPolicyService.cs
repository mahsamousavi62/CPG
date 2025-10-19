using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// Service for managing Polly resilience policies for SOAP services (Retry + Timeout)
/// </summary>
public interface IPollyPolicyService
{
    /// <summary>
    /// Get retry policy for SOAP services with exception handling
    /// </summary>
    IAsyncPolicy GetSoapRetryPolicy(string serviceName = "soap");

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
    /// Get retry policy for SOAP services
    /// </summary>
    public IAsyncPolicy GetSoapRetryPolicy(string serviceName = "soap")
    {
        var retryConfig = _config.Retry.Soap;

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
