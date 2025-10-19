using System.Collections.Generic;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// Configuration for Polly resilience policies (HTTP + SOAP Retry + Timeout)
/// </summary>
public class PolicyConfig
{
    public const string SectionName = "Infrastructure:Polly";

    /// <summary>
    /// Retry policy configuration for HTTP and SOAP
    /// </summary>
    public RetryConfig Retry { get; set; } = new();

    /// <summary>
    /// Timeout policy configuration
    /// </summary>
    public TimeoutConfig Timeout { get; set; } = new();
}

/// <summary>
/// Unified retry configuration for both HTTP and SOAP services
/// </summary>
public class RetryConfig
{
    /// <summary>
    /// SOAP retry configuration
    /// </summary>
    public SoapRetryConfig Soap { get; set; } = new();

    /// <summary>
    /// HTTP retry configuration
    /// </summary>
    public HttpRetryConfig Http { get; set; } = new();
}

/// <summary>
/// SOAP-specific retry configuration
/// </summary>
public class SoapRetryConfig
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

/// <summary>
/// HTTP retry configuration with default and service-specific settings
/// </summary>
public class HttpRetryConfig
{
    /// <summary>
    /// Default retry settings for unnamed HTTP clients
    /// </summary>
    public HttpRetrySettings Default { get; set; } = new();

    /// <summary>
    /// Service-specific retry settings (CharisPay, IdpClient, NeoBank, etc.)
    /// </summary>
    public Dictionary<string, HttpRetrySettings> Services { get; set; } = new();
}

/// <summary>
/// HTTP retry settings for a specific service or default
/// </summary>
public class HttpRetrySettings
{
    /// <summary>
    /// Number of retry attempts (default: 3)
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Base delay in seconds (default: 1)
    /// </summary>
    public int BaseDelaySeconds { get; set; } = 1;

    /// <summary>
    /// Use exponential backoff (true) or linear (false) (default: true)
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

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
