namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents HTTP response information for audit logging
/// </summary>
public sealed record HttpResponseLog
{
    /// <summary>
    /// HTTP status code (200, 404, 500, etc.)
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// HTTP response headers (serialized as JSON)
    /// </summary>
    public string? Header { get; init; }

    /// <summary>
    /// HTTP response body content (may be truncated for large payloads)
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Indicates whether the response was successful (2xx status code)
    /// </summary>
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    private HttpResponseLog()
    {
    }

    public HttpResponseLog(int statusCode, string? header = null, string? body = null)
    {
        StatusCode = statusCode;
        Header = header;
        Body = body;
    }

    /// <summary>
    /// Creates a new instance with truncated body for logging
    /// </summary>
    public HttpResponseLog WithTruncatedBody(int maxLength = 2000)
    {
        if (string.IsNullOrEmpty(Body) || Body.Length <= maxLength)
            return this;

        return this with { Body = Body[..maxLength] + "... (truncated)" };
    }
}
