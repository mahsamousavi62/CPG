using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// Wrapper to log SOAP requests and responses including timeout scenarios
/// Uses IAuditLogService for structured logging with correlation tracking
/// </summary>
public interface ISoapLoggingWrapper
{
    /// <summary>
    /// Execute SOAP call with logging
    /// </summary>
    Task<TResponse> ExecuteWithLoggingAsync<TRequest, TResponse>(
        string serviceName,
        string operationName,
        TRequest request,
        Func<Task<TResponse>> soapCall,
        Domain.SharedKernel.Enums.ProviderTypeInLog providerType = Domain.SharedKernel.Enums.ProviderTypeInLog.Unknown,
        Domain.SharedKernel.Enums.ServiceType serviceType = Domain.SharedKernel.Enums.ServiceType.Soap);
}

public class SoapLoggingWrapper : ISoapLoggingWrapper
{
    private readonly IAuditLogService _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SoapLoggingWrapper(IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
    {
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<TResponse> ExecuteWithLoggingAsync<TRequest, TResponse>(
        string serviceName,
        string operationName,
        TRequest request,
        Func<Task<TResponse>> soapCall,
        Domain.SharedKernel.Enums.ProviderTypeInLog providerType = Domain.SharedKernel.Enums.ProviderTypeInLog.Unknown,
        Domain.SharedKernel.Enums.ServiceType serviceType = Domain.SharedKernel.Enums.ServiceType.Soap)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        // Extract context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        // Extract user context
        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

        // Serialize request
        var requestBody = SerializeObject(request);

        TResponse response = default;

        try
        {
            response = await soapCall();
            stopwatch.Stop();

            var responseBody = SerializeObject(response);

            // Log successful SOAP call
            var log = new ProviderCallLog
            {
                ProviderName = serviceName,
                ProviderType = providerType,
                ServiceType = serviceType,
                ServiceUrl = $"{serviceName}.{operationName}",
                RequestBody = requestBody,
                ResponseBody = responseBody,
                IsSuccess = true,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                IsTimeout = false,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            };

            _auditLogService.LogProviderCall(log);

            return response;
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();

            var log = new ProviderCallLog
            {
                ProviderName = serviceName,
                ProviderType = providerType,
                ServiceType = serviceType,
                ServiceUrl = $"{serviceName}.{operationName}",
                RequestBody = requestBody,
                ResponseBody = $"TIMEOUT after {stopwatch.ElapsedMilliseconds}ms - {ex.Message}",
                IsSuccess = false,
                ErrorCode = "RequestTimeout",
                ErrorMessage = ex.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                IsTimeout = true,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            };

            _auditLogService.LogProviderCall(log);

            throw;
        }
        catch (TimeoutException ex)
        {
            stopwatch.Stop();

            var log = new ProviderCallLog
            {
                ProviderName = serviceName,
                ProviderType = providerType,
                ServiceType = serviceType,
                ServiceUrl = $"{serviceName}.{operationName}",
                RequestBody = requestBody,
                ResponseBody = $"TIMEOUT after {stopwatch.ElapsedMilliseconds}ms - {ex.Message}",
                IsSuccess = false,
                ErrorCode = "TimeoutException",
                ErrorMessage = ex.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                IsTimeout = true,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            };

            _auditLogService.LogProviderCall(log);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var log = new ProviderCallLog
            {
                ProviderName = serviceName,
                ProviderType = providerType,
                ServiceType = serviceType,
                ServiceUrl = $"{serviceName}.{operationName}",
                RequestBody = requestBody,
                ResponseBody = $"ERROR: {ex.GetType().Name} - {ex.Message}",
                IsSuccess = false,
                ErrorCode = ex.GetType().Name,
                ErrorMessage = ex.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                IsTimeout = false,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            };

            _auditLogService.LogProviderCall(log);

            throw;
        }
    }

    private static string SerializeObject(object obj)
    {
        if (obj == null)
            return "[null]";

        try
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            return obj.ToString() ?? "[Could not serialize]";
        }
    }

    private static string TruncateIfNeeded(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength) + $"... [truncated from {text.Length} chars]";
    }
}
