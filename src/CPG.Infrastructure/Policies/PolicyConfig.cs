namespace CPG.Infrastructure.Policies;

/// <summary>
/// Configuration for Polly resilience policies (Retry + Timeout only)
/// </summary>
public class PolicyConfig
{
    public const string SectionName = "Infrastructure:Polly";

    /// <summary>
    /// Retry policy configuration
    /// </summary>
    public RetryPolicyConfig Retry { get; set; } = new();

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

public class TimeoutPolicyConfig
{
    /// <summary>
    /// Timeout in seconds for SOAP service calls (default: 45)
    /// </summary>
    public int SoapTimeoutSeconds { get; set; } = 45;
}
