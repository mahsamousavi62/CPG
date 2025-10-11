namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents the severity level of an audit log entry
/// </summary>
public enum AuditLevel : byte
{
    /// <summary>
    /// Informational log entries for successful operations
    /// </summary>
    Information = 1,

    /// <summary>
    /// Warning log entries for non-critical issues
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error log entries for failures and exceptions
    /// </summary>
    Error = 3
}
