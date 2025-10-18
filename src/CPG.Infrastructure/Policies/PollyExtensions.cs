using Polly;
using Polly.Retry;
using Polly.Timeout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace CPG.Infrastructure.Policies;

public static class PollyExtensions
{
    // برای استفاده در HttpClient
    public static IHttpClientBuilder AddStandardRetryPolicy(
        this IHttpClientBuilder builder,
        int maxRetryAttempts = 3)
    {
        return builder.AddStandardResilienceHandler(options =>
        {
            // فقط Retry - بدون CircuitBreaker
            options.Retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = maxRetryAttempts,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutException>()
                    .HandleResult(response =>
                        response.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                        response.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                        response.StatusCode == (System.Net.HttpStatusCode)429),
                OnRetry = args =>
                {
                    var logger = args.Context.ServiceProvider?.GetService<ILogger<PollyExtensions>>();
                    logger?.LogWarning("[Polly Retry] Attempt {AttemptNumber} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            };

            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        });
    }

    // برای استفاده دستی در SOAP یا کدهای دیگر
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        ILogger logger,
        int maxRetryAttempts = 3,
        string serviceName = null)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<System.ServiceModel.CommunicationException>()
            .WaitAndRetryAsync(
                maxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogWarning(exception,
                        "[Polly Retry] {ServiceName} - Retry {RetryCount}/{MaxRetryAttempts} after {Delay}ms",
                        serviceName, retryCount, maxRetryAttempts, timeSpan.TotalMilliseconds);
                });

        return await retryPolicy.ExecuteAsync(operation);
    }
}
