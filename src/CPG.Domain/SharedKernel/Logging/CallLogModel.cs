using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

public class CallLogModel
{
    public ServiceType? ServiceType { get; set; }

    public AuditType? AuditType { get; set; }

    public string? RequestBody { get; set; }

    public bool? ServiceCallStatus { get; set; }
    public string? ServiceCallUrl { get; set; }
    public DateTime ServiceCallDate { get; set; }
    public string? ResponseBody { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorType { get; set; }

    public DateTime CreationDate { get; set; }

    public long CreationUserId { get; set; }

    public ProviderTypeInLog? ProviderType { get; set; }
    
    public string CorrolationId { get; set; }
}
