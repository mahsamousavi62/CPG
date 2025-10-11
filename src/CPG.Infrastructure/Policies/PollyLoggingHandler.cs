using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// DelegatingHandler to log complete Request and Response including timeout scenarios
/// Uses IAuditLogService for structured logging with correlation tracking
/// </summary>
public class PollyLoggingHandler : DelegatingHandler
{
    private readonly IAuditLogService _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PollyLoggingHandler(IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
    {
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            // Log successful provider call with structured logging
            await LogProviderCallAsync(request, response, startTime, stopwatch.ElapsedMilliseconds, isTimeout: false);

            return response;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // This is a timeout (not user cancellation)
            stopwatch.Stop();
            await LogProviderTimeoutAsync(request, ex, startTime, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            // Also log timeout for OperationCanceledException
            stopwatch.Stop();
            await LogProviderTimeoutAsync(request, ex, startTime, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            // Log other exceptions
            stopwatch.Stop();
            await LogProviderErrorAsync(request, ex, startTime, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task LogProviderCallAsync(
        HttpRequestMessage request,
        HttpResponseMessage response,
        DateTime startTime,
        long durationMs,
        bool isTimeout)
    {
        var requestBody = request.Content != null ? await request.Content.ReadAsStringAsync() : null;
        var responseBody = response.Content != null ? await response.Content.ReadAsStringAsync() : null;

        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        // Extract user context from claims
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

        var log = new ProviderCallLog
        {
            ProviderName = ExtractProviderNameFromUri(request.RequestUri?.ToString() ?? ""),
            ProviderType = Domain.SharedKernel.Enums.ProviderTypeInLog.Unknown, // Will be overridden by specific providers
            ServiceType = Domain.SharedKernel.Enums.ServiceType.Http,
            ServiceUrl = request.RequestUri?.ToString() ?? "",
            RequestBody = requestBody,
            ResponseBody = responseBody,
            IsSuccess = response.IsSuccessStatusCode,
            ErrorCode = !response.IsSuccessStatusCode ? ((int)response.StatusCode).ToString() : null,
            ErrorMessage = !response.IsSuccessStatusCode ? response.ReasonPhrase : null,
            StartDateTime = startTime,
            EndDateTime = startTime.AddMilliseconds(durationMs),
            DurationMs = durationMs,
            IsTimeout = isTimeout,
            UserId = userId,
            CompanyId = companyId,
            ApplicationId = applicationId,
            CorrelationId = correlationId,
            RequestId = requestId
        };

        _auditLogService.LogProviderCall(log);
    }

    private async Task LogProviderTimeoutAsync(
        HttpRequestMessage request,
        Exception exception,
        DateTime startTime,
        long durationMs)
    {
        var requestBody = request.Content != null ? await request.Content.ReadAsStringAsync() : null;

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

        var log = new ProviderCallLog
        {
            ProviderName = ExtractProviderNameFromUri(request.RequestUri?.ToString() ?? ""),
            ProviderType = Domain.SharedKernel.Enums.ProviderTypeInLog.Unknown,
            ServiceType = Domain.SharedKernel.Enums.ServiceType.Http,
            ServiceUrl = request.RequestUri?.ToString() ?? "",
            RequestBody = requestBody,
            ResponseBody = $"TIMEOUT after {durationMs}ms - {exception.Message}",
            IsSuccess = false,
            ErrorCode = "RequestTimeout",
            ErrorMessage = exception.Message,
            StartDateTime = startTime,
            EndDateTime = startTime.AddMilliseconds(durationMs),
            DurationMs = durationMs,
            IsTimeout = true,
            UserId = userId,
            CompanyId = companyId,
            ApplicationId = applicationId,
            CorrelationId = correlationId,
            RequestId = requestId
        };

        _auditLogService.LogProviderCall(log);
    }

    private async Task LogProviderErrorAsync(
        HttpRequestMessage request,
        Exception exception,
        DateTime startTime,
        long durationMs)
    {
        var requestBody = request.Content != null ? await request.Content.ReadAsStringAsync() : null;

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

        var log = new ProviderCallLog
        {
            ProviderName = ExtractProviderNameFromUri(request.RequestUri?.ToString() ?? ""),
            ProviderType = Domain.SharedKernel.Enums.ProviderTypeInLog.Unknown,
            ServiceType = Domain.SharedKernel.Enums.ServiceType.Http,
            ServiceUrl = request.RequestUri?.ToString() ?? "",
            RequestBody = requestBody,
            ResponseBody = $"ERROR: {exception.GetType().Name} - {exception.Message}",
            IsSuccess = false,
            ErrorCode = exception.GetType().Name,
            ErrorMessage = exception.Message,
            StartDateTime = startTime,
            EndDateTime = startTime.AddMilliseconds(durationMs),
            DurationMs = durationMs,
            IsTimeout = false,
            UserId = userId,
            CompanyId = companyId,
            ApplicationId = applicationId,
            CorrelationId = correlationId,
            RequestId = requestId
        };

        _auditLogService.LogProviderCall(log);
    }

    private static string ExtractProviderNameFromUri(string uri)
    {
        try
        {
            var url = new Uri(uri);
            return url.Host;
        }
        catch
        {
            return "Unknown";
        }
    }
}
