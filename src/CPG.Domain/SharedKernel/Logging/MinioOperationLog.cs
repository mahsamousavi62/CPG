using System;

namespace CPG.Domain.SharedKernel.Logging;

/// <summary>
/// Log model for MinIO file storage operations
/// </summary>
public class MinioOperationLog
{
    public required string OperationType { get; set; } // PutObject, GetObject, DeleteObject, ListObjects
    public required string BucketName { get; set; }
    public required string ObjectName { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? ContentType { get; set; }
    public string? EntityType { get; set; } // Company, Bank, Provider, Application, PaymentReceipt
    public required bool IsSuccess { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required long DurationMs { get; set; }

    // User context
    public long? UserId { get; set; }
    public long? CompanyId { get; set; }
    public long? ApplicationId { get; set; }

    // Request context
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}
