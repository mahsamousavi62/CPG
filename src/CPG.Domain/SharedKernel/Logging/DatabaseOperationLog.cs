using System;

namespace CPG.Domain.SharedKernel.Logging;

/// <summary>
/// Log model for database operations
/// </summary>
public class DatabaseOperationLog
{
    public required string OperationType { get; set; } // Query, Insert, Update, Delete, SaveChanges
    public required string ContextType { get; set; } // ReadDbContext, WriteDbContext
    public string? EntityType { get; set; }
    public int? EntityCount { get; set; }
    public string? SqlCommand { get; set; }
    public required bool IsSuccess { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required long DurationMs { get; set; }

    // Performance metrics
    public int? RowsAffected { get; set; }
    public bool IsSlow { get; set; } // Flag for slow queries (> 1000ms)

    // User context
    public long? UserId { get; set; }
    public long? CompanyId { get; set; }
    public long? ApplicationId { get; set; }

    // Request context
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}
