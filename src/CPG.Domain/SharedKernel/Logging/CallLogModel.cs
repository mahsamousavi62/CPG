using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

public class CallLogModel
{
	public string RequestId { get; set; }
	public string LogId { get; set; }
	public string AuditLevel { get; set; }
	public AuditType? AuditType { get; set; }
	public string ServiceName { get; set; }
	public string ProviderName { get; set; }
	public ProviderTypeInLog? ProviderType { get; set; }
	public string RequestUri { get; set; }
	public string RequestHeader { get; set; }
	public string RequestBody { get; set; }
	public int? ResponseStatusCode { get; set; }
	public string ResponseHeader { get; set; }
	public string ResponseBody { get; set; }
	public string ErrorCode { get; set; }
	public string ErrorType { get; set; }
	public string StackTrace { get; set; }
	public long? ApplicationId { get; set; }
	public long? UserId { get; set; }
	public long? CompanyId { get; set; }
	public string IpAddress { get; set; }
	public string UserAgent { get; set; }
	public bool IsSucceeded { get; set; }
	public DateTime StartDateTime { get; set; }
	public DateTime EndDateTime { get; set; }
	public long DurationMs { get; set; }
	public ServiceType? ServiceType { get; init; }
	// ===== Legacy/Compatibility Fields =====

	/// <summary>
	/// Legacy field - mapped to IsSucceeded
	/// </summary>
	[Obsolete("Use IsSucceeded instead")]
	public bool? ServiceCallStatus
	{
		get => IsSucceeded;
		set => IsSucceeded = value ?? false;
	}

	/// <summary>
	/// Legacy field - mapped to RequestUri
	/// </summary>
	[Obsolete("Use RequestUri instead")]
	public string? ServiceCallUrl
	{
		get => RequestUri;
		set => RequestUri = value;
	}

	/// <summary>
	/// Legacy field - mapped to StartDateTime
	/// </summary>
	[Obsolete("Use StartDateTime instead")]
	public DateTime ServiceCallDate
	{
		get => StartDateTime;
		set => StartDateTime = value;
	}

	/// <summary>
	/// Legacy field - mapped to StartDateTime
	/// </summary>
	[Obsolete("Use StartDateTime instead")]
	public DateTime CreationDate
	{
		get => StartDateTime;
		set => StartDateTime = value;
	}

	/// <summary>
	/// Legacy field - mapped to UserId
	/// </summary>
	[Obsolete("Use UserId instead")]
	public long CreationUserId
	{
		get => UserId ?? 0;
		set => UserId = value > 0 ? value : null;
	}
}
