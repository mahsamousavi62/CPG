using System;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents error information for audit logging
/// </summary>
public sealed record ErrorInfo
{
    /// <summary>
    /// Error code (e.g., HTTP status code, business error code)
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Error type or category (e.g., "ValidationError", "TimeoutError")
    /// </summary>
    public string? ErrorType { get; init; }

    /// <summary>
    /// Stack trace for exceptions (truncated for logging)
    /// </summary>
    public string? StackTrace { get; init; }

    /// <summary>
    /// Exception message
    /// </summary>
    public string? Message { get; init; }

    private ErrorInfo()
    {
    }

    public ErrorInfo(
        string? errorCode = null,
        string? errorType = null,
        string? stackTrace = null,
        string? message = null)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;
        StackTrace = stackTrace;
        Message = message;
    }

    /// <summary>
    /// Creates error info from an exception
    /// </summary>
    public static ErrorInfo FromException(Exception exception, string? errorCode = null)
    {
        return new ErrorInfo(
            errorCode: errorCode,
            errorType: exception.GetType().Name,
            stackTrace: exception.StackTrace,
            message: exception.Message);
    }

    /// <summary>
    /// Creates error info from HTTP error
    /// </summary>
    public static ErrorInfo FromHttpError(int statusCode, string? reasonPhrase = null)
    {
        return new ErrorInfo(
            errorCode: statusCode.ToString(),
            errorType: "HttpError",
            message: reasonPhrase);
    }

    /// <summary>
    /// Creates error info for business logic errors
    /// </summary>
    public static ErrorInfo FromBusinessError(string errorCode, string message)
    {
        return new ErrorInfo(
            errorCode: errorCode,
            errorType: "BusinessError",
            message: message);
    }

    /// <summary>
    /// Creates a new instance with truncated stack trace for logging
    /// </summary>
    public ErrorInfo WithTruncatedStackTrace(int maxLength = 2000)
    {
        if (string.IsNullOrEmpty(StackTrace) || StackTrace.Length <= maxLength)
            return this;

        return this with { StackTrace = StackTrace[..maxLength] + "... (truncated)" };
    }
}
