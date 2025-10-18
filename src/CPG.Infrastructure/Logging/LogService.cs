using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Logging;

public partial class LogService(ILogger<LogService> logger, IHttpContextAccessor httpContextAccessor) : ILogService
{
	private readonly ILogger<LogService> _logger = logger;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	public string ServiceName { get; set; }
	public Enums.ServiceType ServiceType { get; set; }
	public Enums.ProviderTypeInLog ProviderTypeInLog { get; set; }
	public void LogServiceCall(CallLogModel logModel)
	{
		// Enrich with context from HttpContext if not provided
		EnrichLogModelFromHttpContext(logModel);

		// Generate LogId if not provided (format: {Id}-{Domain}-{Abbreviation})
		if (string.IsNullOrEmpty(logModel.LogId))
		{
			logModel.LogId = GenerateLogId(logModel);
		}

		logModel.RequestBody = SanitizeSensitiveData(logModel.RequestBody);
		logModel.ResponseBody = SanitizeSensitiveData(logModel.ResponseBody);
		logModel.RequestHeader = SanitizeSensitiveData(logModel.RequestHeader);
		logModel.ResponseHeader = SanitizeSensitiveData(logModel.ResponseHeader);

		var logLevel = DetermineLogLevel(logModel);

		using (LogContext.PushProperty("CallLog", logModel, true))
		{
			var messageTemplate = BuildLogMessage(logModel);

			switch (logLevel)
			{
				case LogLevel.Information:
				_logger.LogInformation(messageTemplate);
				break;
				case LogLevel.Warning:
				_logger.LogWarning(messageTemplate);
				break;
				case LogLevel.Error:
				if (!string.IsNullOrEmpty(logModel.StackTrace))
				{
					_logger.LogError(messageTemplate + "\nStackTrace: {StackTrace}", logModel.StackTrace);
				}
				else
				{
					_logger.LogError(messageTemplate);
				}
				break;
			}
		}
	}

	public void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString)
	{
		var logModel = new CallLogModel
		{
			RequestBody = System.Text.Json.JsonSerializer.Serialize(request),
			ResponseBody = resString,
			StartDateTime = DateTime.UtcNow,
			EndDateTime = DateTime.UtcNow,
			DurationMs = 0,
			RequestUri = request.Uri,
			ResponseStatusCode = (int)response.StatusCode,
			IsSucceeded = response.StatusCode == System.Net.HttpStatusCode.OK,
			ServiceType = request.Service,
			ServiceName = request.Uri,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
			ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
			ProviderType = request.Provider,
			ProviderName = request.Provider.ToString(),
			AuditType = Enums.AuditType.Provider
		};

		LogServiceCall(logModel);
	}

	public void AddServiceCallLog(string request, string response, short status, string message)
	{
		var logModel = new CallLogModel
		{
			RequestBody = request,
			ResponseBody = response,
			StartDateTime = DateTime.UtcNow,
			EndDateTime = DateTime.UtcNow,
			DurationMs = 0,
			ServiceName = ServiceName,
			RequestUri = ServiceName,
			ResponseStatusCode = status,
			IsSucceeded = status == 0,
			ServiceType = ServiceType,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = status < 0 ? message : null,
			ErrorType = status < 0 ? status.ToString() : null,
			ProviderType = ProviderTypeInLog,
			ProviderName = ProviderTypeInLog.ToString(),
			AuditType = Enums.AuditType.Provider
		};

		LogServiceCall(logModel);
	}

	public async Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response)
	{
		string resString = await response.Content.ReadAsStringAsync();

		var logModel = new CallLogModel
		{
			RequestBody = System.Text.Json.JsonSerializer.Serialize(request),
			ResponseBody = resString,
			StartDateTime = DateTime.UtcNow,
			EndDateTime = DateTime.UtcNow,
			DurationMs = 0,
			RequestUri = request.Uri,
			ResponseStatusCode = (int)response.StatusCode,
			IsSucceeded = response.StatusCode == System.Net.HttpStatusCode.OK,
			ServiceType = request.Service,
			ServiceName = request.Uri,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
			ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
			ProviderType = request.ProviderTypeInLog,
			ProviderName = request.ProviderTypeInLog.ToString(),
			AuditType = Enums.AuditType.Provider
		};

		LogServiceCall(logModel);
	}

	public void AddTimeoutLog<TBody>(HttpProviderRequest<TBody> request, Exception exception, long durationMs)
	{
		var logModel = new CallLogModel
		{
			RequestBody = System.Text.Json.JsonSerializer.Serialize(request),
			ResponseBody = $"TIMEOUT after {durationMs}ms - {exception.Message}",
			StartDateTime = DateTime.UtcNow.AddMilliseconds(-durationMs),
			EndDateTime = DateTime.UtcNow,
			DurationMs = durationMs,
			RequestUri = request.Uri,
			IsSucceeded = false,
			ServiceType = request.Service,
			ServiceName = request.Uri,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = "RequestTimeout",
			ErrorType = "Timeout",
			ProviderType = request.Provider,
			ProviderName = request.Provider.ToString(),
			AuditType = Enums.AuditType.Provider,
			AuditLevel = "Error",
			StackTrace = exception.StackTrace
		};

		LogServiceCall(logModel);
	}

	public void AddSoapCallLog<TRequest, TResponse>(TRequest request, TResponse response, string serviceName, short status, string message)
	{
		var logModel = new CallLogModel
		{
			RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request),
			ResponseBody = Newtonsoft.Json.JsonConvert.SerializeObject(response),
			StartDateTime = DateTime.UtcNow,
			EndDateTime = DateTime.UtcNow,
			DurationMs = 0,
			ServiceName = serviceName,
			RequestUri = serviceName,
			ResponseStatusCode = status,
			IsSucceeded = status == 0,
			ServiceType = ServiceType,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = status < 0 ? message : null,
			ErrorType = status < 0 ? status.ToString() : null,
			ProviderType = ProviderTypeInLog,
			ProviderName = ProviderTypeInLog.ToString(),
			AuditType = Enums.AuditType.Provider
		};

		LogServiceCall(logModel);
	}

	public void AddSoapTimeoutLog<TRequest>(TRequest request, string serviceName, Exception exception, long durationMs)
	{
		var logModel = new CallLogModel
		{
			RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request),
			ResponseBody = $"TIMEOUT after {durationMs}ms - {exception.Message}",
			StartDateTime = DateTime.UtcNow.AddMilliseconds(-durationMs),
			EndDateTime = DateTime.UtcNow,
			DurationMs = durationMs,
			ServiceName = serviceName,
			RequestUri = serviceName,
			IsSucceeded = false,
			ServiceType = ServiceType,
			UserId = GetUserIdFromHttpContext(),
			ErrorCode = "RequestTimeout",
			ErrorType = "Timeout",
			ProviderType = ProviderTypeInLog,
			ProviderName = ProviderTypeInLog.ToString(),
			AuditType = Enums.AuditType.Provider,
			AuditLevel = "Error",
			StackTrace = exception.StackTrace
		};

		LogServiceCall(logModel);
	}

	private void EnrichLogModelFromHttpContext(CallLogModel logModel)
	{
		var httpContext = _httpContextAccessor.HttpContext;
		if (httpContext == null) return;

		logModel.RequestId ??= httpContext.Items["RequestId"]?.ToString() ?? httpContext.TraceIdentifier;

		logModel.IpAddress ??= httpContext.Request.GetClientIpAddress();

		logModel.UserAgent ??= httpContext.Request.Headers.UserAgent.ToString();

		if (httpContext.User?.Claims != null)
		{
			if (logModel.UserId == null)
			{
				var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
				if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
					logModel.UserId = uid;
			}

			if (logModel.CompanyId == null)
			{
				var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
				if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
					logModel.CompanyId = cid;
			}

			if (logModel.ApplicationId == null)
			{
				var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
				if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
					logModel.ApplicationId = aid;
			}
		}
	}

	private string GenerateLogId(CallLogModel logModel)
	{
		var id = Guid.NewGuid().ToString("N")[..8];
		var domain = logModel.AuditType?.ToString() ?? "Unknown";
		var abbreviation = GetErrorAbbreviation(logModel);

		return $"{id}-{domain}-{abbreviation}";
	}

	private string GetErrorAbbreviation(CallLogModel logModel)
	{
		if (logModel.IsSucceeded)
			return "OK";

		if (!string.IsNullOrEmpty(logModel.ErrorType))
		{

			var words = Regex.Split(logModel.ErrorType, @"(?<!^)(?=[A-Z])");
			return string.Join("", words.Select(w => w.Length > 0 ? w[0] : ' ')).ToUpper();
		}

		return "ERR";
	}

	private LogLevel DetermineLogLevel(CallLogModel logModel)
	{
		if (!string.IsNullOrEmpty(logModel.AuditLevel))
		{
			return logModel.AuditLevel.ToLowerInvariant() switch
			{
				"information" => LogLevel.Information,
				"warning" => LogLevel.Warning,
				"error" => LogLevel.Error,
				_ => logModel.IsSucceeded ? LogLevel.Information : LogLevel.Warning
			};
		}

		// If StackTrace is present, it's an error
		if (!string.IsNullOrEmpty(logModel.StackTrace))
			return LogLevel.Error;

		// Default based on success status
		return logModel.IsSucceeded ? LogLevel.Information : LogLevel.Warning;
	}

	private string BuildLogMessage(CallLogModel logModel)
	{
		var auditTypeStr = logModel.AuditType?.ToString() ?? "Unknown";
		var statusStr = logModel.IsSucceeded ? "SUCCESS" : "FAILED";

		if (logModel.AuditType == Enums.AuditType.Provider)
		{
			var providerInfo = !string.IsNullOrEmpty(logModel.ProviderName)
				? $"{logModel.ProviderName} ({logModel.ProviderType})"
				: logModel.ProviderType?.ToString() ?? "Unknown";

			if (!logModel.IsSucceeded && !string.IsNullOrEmpty(logModel.ErrorCode))
			{
				return $"[{auditTypeStr}] {statusStr} - {providerInfo} {logModel.ServiceType} - " +
					   $"Duration: {logModel.DurationMs}ms, URI: {logModel.RequestUri}, " +
					   $"Error: {logModel.ErrorCode} - {logModel.ErrorType}";
			}

			return $"[{auditTypeStr}] {statusStr} - {providerInfo} {logModel.ServiceType} - " +
				   $"Duration: {logModel.DurationMs}ms, URI: {logModel.RequestUri}";
		}

		if (logModel.AuditType == Enums.AuditType.Client)
		{
			if (!logModel.IsSucceeded && !string.IsNullOrEmpty(logModel.ErrorCode))
			{
				return $"[{auditTypeStr}] {statusStr} - {logModel.ServiceName} - " +
					   $"Status: {logModel.ResponseStatusCode}, Duration: {logModel.DurationMs}ms, " +
					   $"Error: {logModel.ErrorCode}";
			}

			return $"[{auditTypeStr}] {statusStr} - {logModel.ServiceName} - " +
				   $"Status: {logModel.ResponseStatusCode}, Duration: {logModel.DurationMs}ms";
		}

		// Default message
		return $"[{auditTypeStr}] {statusStr} - {logModel.ServiceName} - Duration: {logModel.DurationMs}ms";
	}

	private string? SanitizeSensitiveData(string? data)
	{
		if (string.IsNullOrEmpty(data))
			return data;

		try
		{
			return MyRegex().Replace(data, Constants.Replaceformat);
		}
		catch
		{
			return data;
		}
	}

	private long? GetUserIdFromHttpContext()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		if (httpContext?.User?.Claims == null) return null;

		var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
		if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var userId))
			return userId;

		return null;
	}

	[GeneratedRegex(Constants.Pattern)]
	private static partial Regex MyRegex();

	public void LogMinioOperation(MinioOperationLog log)
	{
		var logModel = new CallLogModel
		{
			// Core identifiers
			RequestId = log.RequestId,
			LogId = $"{Guid.NewGuid().ToString("N")[..8]}-MinIO-{(log.IsSuccess ? "OK" : "ERR")}",

			// Service information
			ServiceName = $"MinIO:{log.OperationType}",
			ProviderName = "MinIO",
			ProviderType = Enums.ProviderTypeInLog.FileStorage,
			ServiceType = Enums.ServiceType.FileStorage,

			// Request/Response information
			RequestUri = $"{log.BucketName}/{log.ObjectName}",
			RequestBody = System.Text.Json.JsonSerializer.Serialize(new
			{
				log.BucketName,
				log.ObjectName,
				log.FileSizeBytes,
				log.ContentType,
				log.EntityType
			}),
			ResponseBody = log.IsSuccess
				? $"Success - Size: {log.FileSizeBytes} bytes"
				: $"Failed - {log.ErrorMessage}",
			ResponseStatusCode = log.IsSuccess ? 200 : 500,

			// Status and timing
			IsSucceeded = log.IsSuccess,
			StartDateTime = log.StartDateTime,
			EndDateTime = log.EndDateTime,
			DurationMs = log.DurationMs,

			// Error information
			ErrorCode = log.ErrorCode,
			ErrorType = log.ErrorMessage,

			// Context
			UserId = log.UserId,
			CompanyId = log.CompanyId,
			ApplicationId = log.ApplicationId,
			AuditType = Enums.AuditType.Provider,
			AuditLevel = log.IsSuccess ? "Information" : "Error"
		};

		LogServiceCall(logModel);
	}

	public void LogDatabaseOperation(DatabaseOperationLog log)
	{
		var logModel = new CallLogModel
		{
			RequestId = log.RequestId,
			LogId = $"{Guid.NewGuid().ToString("N")[..8]}-DB-{(log.IsSuccess ? "OK" : "ERR")}",

			ServiceName = $"Database:{log.OperationType}",
			ProviderName = log.ContextType,
			ProviderType = Enums.ProviderTypeInLog.Database,
			ServiceType = Enums.ServiceType.Database,

			RequestUri = $"{log.ContextType}.{log.EntityType}",
			RequestBody = log.SqlCommand,
			ResponseBody = log.IsSuccess
				? $"Success - Rows: {log.RowsAffected ?? 0}"
				: $"Failed - {log.ErrorMessage}",
			ResponseStatusCode = log.IsSuccess ? 200 : 500,

			IsSucceeded = log.IsSuccess,
			StartDateTime = log.StartDateTime,
			EndDateTime = log.EndDateTime,
			DurationMs = log.DurationMs,

			ErrorCode = log.ErrorCode,
			ErrorType = log.ErrorMessage,

			UserId = log.UserId,
			CompanyId = log.CompanyId,
			ApplicationId = log.ApplicationId,
			AuditType = Enums.AuditType.Database,
			AuditLevel = log.IsSlow ? "Warning" : (log.IsSuccess ? "Information" : "Error")
		};

		LogServiceCall(logModel);
	}

	public void LogProviderCall(ProviderCallLog log)
	{
		var logModel = new CallLogModel
		{
			RequestId = log.RequestId,
			LogId = $"{Guid.NewGuid().ToString("N")[..8]}-Provider-{(log.IsSuccess ? "OK" : "ERR")}",

			// Service information
			ServiceName = log.ServiceUrl,
			ProviderName = log.ProviderName,
			ProviderType = log.ProviderType,
			ServiceType = log.ServiceType,

			// Request/Response information
			RequestUri = log.ServiceUrl,
			RequestBody = log.RequestBody,
			ResponseBody = log.ResponseBody,
			ResponseStatusCode = log.IsSuccess ? 200 : 500,

			// Status and timing
			IsSucceeded = log.IsSuccess,
			StartDateTime = log.StartDateTime,
			EndDateTime = log.EndDateTime,
			DurationMs = log.DurationMs,

			// Error information
			ErrorCode = log.ErrorCode,
			ErrorType = log.ErrorMessage,

			// Context
			UserId = log.UserId,
			CompanyId = log.CompanyId,
			ApplicationId = log.ApplicationId,
			AuditType = Enums.AuditType.Provider,
			AuditLevel = log.IsTimeout ? "Error" : (log.IsSuccess ? "Information" : "Warning")
		};

		LogServiceCall(logModel);
	}

}
