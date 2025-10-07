namespace CPG.Infrastructure.Policies;

/// <summary>
/// Configuration for Polly resilience policies
/// </summary>
public class PolicyConfig
{
    public const string SectionName = "Infrastructure:Polly";

    /// <summary>
    /// Retry policy configuration
    /// </summary>
    public RetryPolicyConfig Retry { get; set; } = new();

    /// <summary>
    /// Circuit breaker policy configuration
    /// </summary>
    public CircuitBreakerPolicyConfig CircuitBreaker { get; set; } = new();

    /// <summary>
    /// Timeout policy configuration
    /// </summary>
    public TimeoutPolicyConfig Timeout { get; set; } = new();
}

public class RetryPolicyConfig
{
    /// <summary>
    /// Maximum number of retry attempts (default: 3)
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Base delay in seconds for exponential backoff (default: 2)
    /// </summary>
    public int BaseDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Enable jitter for retry delays to prevent thundering herd (default: true)
    /// </summary>
    public bool UseJitter { get; set; } = true;
}

public class CircuitBreakerPolicyConfig
{
    /// <summary>
    /// Enable circuit breaker (default: true)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Number of consecutive failures before opening circuit (default: 5)
    /// </summary>
    public int FailureThreshold { get; set; } = 5;

    /// <summary>
    /// Sampling duration in seconds to track failures (default: 30)
    /// </summary>
    public int SamplingDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Minimum throughput (requests) before circuit can break (default: 7)
    /// </summary>
    public int MinimumThroughput { get; set; } = 7;

    /// <summary>
    /// Duration in seconds to keep circuit open (default: 60)
    /// </summary>
    public int DurationOfBreakSeconds { get; set; } = 60;
}

public class TimeoutPolicyConfig
{
    /// <summary>
    /// Timeout in seconds for HTTP requests (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Timeout in seconds for SOAP service calls (default: 45)
    /// </summary>
    public int SoapTimeoutSeconds { get; set; } = 45;
}
