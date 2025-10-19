namespace CPG.Infrastructure.Policies;

/// <summary>
/// Configuration for Polly resilience policies (unified for HTTP and SOAP)
/// </summary>
public class PolicyConfig
{
    public const string SectionName = "Infrastructure:Polly";

    /// <summary>
    /// Unified retry policy configuration for all services (HTTP and SOAP)
    /// </summary>
    public RetryConfig Retry { get; set; } = new();

    /// <summary>
    /// Timeout policy configuration
    /// </summary>
    public TimeoutConfig Timeout { get; set; } = new();
}

/// <summary>
/// Unified retry configuration for all HTTP and SOAP services
/// </summary>
public class RetryConfig
{
    /// <summary>
    /// Maximum number of retry attempts (default: 3)
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Base delay in seconds (default: 1)
    /// </summary>
    public int BaseDelaySeconds { get; set; } = 1;

    /// <summary>
    /// Use exponential backoff (true) or linear (false) (default: true)
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Enable jitter for retry delays to prevent thundering herd (default: true)
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Maximum jitter in milliseconds (default: 1000)
    /// </summary>
    public int MaxJitterMilliseconds { get; set; } = 1000;
}

/// <summary>
/// Timeout configuration
/// </summary>
public class TimeoutConfig
{
    /// <summary>
    /// Timeout in seconds for SOAP service calls (default: 45)
    /// </summary>
    public int SoapTimeoutSeconds { get; set; } = 45;
}
