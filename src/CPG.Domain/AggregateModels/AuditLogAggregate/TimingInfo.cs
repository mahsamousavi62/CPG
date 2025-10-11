using System;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents timing information for audit logging
/// </summary>
public sealed record TimingInfo
{
    /// <summary>
    /// Start timestamp of the operation
    /// </summary>
    public DateTime StartDateTime { get; init; }

    /// <summary>
    /// End timestamp of the operation
    /// </summary>
    public DateTime EndDateTime { get; init; }

    /// <summary>
    /// Duration of the operation in milliseconds
    /// </summary>
    public long DurationMs { get; init; }

    private TimingInfo()
    {
    }

    public TimingInfo(DateTime startDateTime, DateTime endDateTime)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        DurationMs = (long)(endDateTime - startDateTime).TotalMilliseconds;
    }

    public TimingInfo(DateTime startDateTime, long durationMs)
    {
        StartDateTime = startDateTime;
        DurationMs = durationMs;
        EndDateTime = startDateTime.AddMilliseconds(durationMs);
    }

    /// <summary>
    /// Creates timing info from start time and calculates duration to now
    /// </summary>
    public static TimingInfo FromStart(DateTime startDateTime)
    {
        return new TimingInfo(startDateTime, DateTime.UtcNow);
    }

    /// <summary>
    /// Creates timing info with duration
    /// </summary>
    public static TimingInfo WithDuration(DateTime startDateTime, long durationMs)
    {
        return new TimingInfo(startDateTime, durationMs);
    }

    /// <summary>
    /// Indicates if the operation exceeded a timeout threshold
    /// </summary>
    public bool ExceededThreshold(long thresholdMs)
    {
        return DurationMs > thresholdMs;
    }
}
