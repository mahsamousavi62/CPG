using System;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents HTTP request information for audit logging
/// </summary>
public sealed record HttpRequestLog
{
    /// <summary>
    /// The URI of the HTTP request
    /// </summary>
    public string Uri { get; init; }

    /// <summary>
    /// HTTP request headers (serialized as JSON)
    /// </summary>
    public string? Header { get; init; }

    /// <summary>
    /// HTTP request body content (may be truncated for large payloads)
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// HTTP method (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string? Method { get; init; }

    /// <summary>
    /// Query string parameters
    /// </summary>
    public string? QueryString { get; init; }

    private HttpRequestLog()
    {
    }

    public HttpRequestLog(string uri, string? header = null, string? body = null, string? method = null, string? queryString = null)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        Header = header;
        Body = body;
        Method = method;
        QueryString = queryString;
    }

    /// <summary>
    /// Creates a new instance with truncated body for logging
    /// </summary>
    public HttpRequestLog WithTruncatedBody(int maxLength = 3000)
    {
        if (string.IsNullOrEmpty(Body) || Body.Length <= maxLength)
            return this;

        return this with { Body = Body[..maxLength] + "... (truncated)" };
    }
}
