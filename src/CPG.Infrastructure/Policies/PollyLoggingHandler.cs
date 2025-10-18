using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;

namespace CPG.Infrastructure.Policies;

public class PollyLoggingHandler : DelegatingHandler
{
    private readonly ILogger<PollyLoggingHandler> _logger;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PollyLoggingHandler(
        ILogger<PollyLoggingHandler> logger,
        ILogService logService,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();
        string requestBody = null;
        string responseBody = null;

        try
        {
            // ذخیره request body
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            // ذخیره response body
            if (response.Content != null)
            {
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            }

            // لاگ فقط در صورت خطا یا timeout طولانی
            if (!response.IsSuccessStatusCode || stopwatch.ElapsedMilliseconds > 25000)
            {
                LogRequestResponse(request, response, requestBody, responseBody,
                    stopwatch.ElapsedMilliseconds, requestId);
            }

            return response;
        }
        catch (TimeoutException ex)
        {
            stopwatch.Stop();

            // لاگ کامل timeout
            _logger.LogError(ex,
                "[{RequestId}] HTTP TIMEOUT after {Duration}ms | {Method} {Uri} | Request: {RequestBody}",
                requestId, stopwatch.ElapsedMilliseconds, request.Method, request.RequestUri,
                TruncateBody(requestBody, 3000));

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "[{RequestId}] HTTP FAILED after {Duration}ms | {Method} {Uri}",
                requestId, stopwatch.ElapsedMilliseconds, request.Method, request.RequestUri);

            throw;
        }
    }

    private void LogRequestResponse(HttpRequestMessage request, HttpResponseMessage response,
        string requestBody, string responseBody, long durationMs, string requestId)
    {
        _logger.LogWarning(
            "[{RequestId}] HTTP {StatusCode} in {Duration}ms | {Method} {Uri} | Request: {RequestBody} | Response: {ResponseBody}",
            requestId, (int)response.StatusCode, durationMs, request.Method, request.RequestUri,
            TruncateBody(requestBody, 3000), TruncateBody(responseBody, 2000));
    }

    private static string TruncateBody(string body, int maxLength)
    {
        if (string.IsNullOrEmpty(body) || body.Length <= maxLength)
            return body;

        return body.Substring(0, maxLength) + $"... [بریده شده: {body.Length} کاراکتر]";
    }
}
