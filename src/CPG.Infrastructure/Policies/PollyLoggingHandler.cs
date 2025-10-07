using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// DelegatingHandler to log complete Request and Response including timeout scenarios
/// Uses project's ILogService for consistent logging format
/// </summary>
public class PollyLoggingHandler : DelegatingHandler
{
    private readonly ILogService _logService;

    public PollyLoggingHandler(ILogService logService)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Normal flow - let response happen and log via ILogService
        // Only catch timeout to log request body
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            return response;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // This is a timeout (not user cancellation)
            stopwatch.Stop();

            // Create HttpProviderRequest from HttpRequestMessage for logging
            var httpProviderRequest = await CreateHttpProviderRequestFromHttpRequestMessage(request);
            if (httpProviderRequest != null)
            {
                _logService.AddTimeoutLog(httpProviderRequest, ex, stopwatch.ElapsedMilliseconds);
            }

            throw;
        }
        catch (OperationCanceledException ex)
        {
            // Also log timeout for OperationCanceledException
            stopwatch.Stop();

            var httpProviderRequest = await CreateHttpProviderRequestFromHttpRequestMessage(request);
            if (httpProviderRequest != null)
            {
                _logService.AddTimeoutLog(httpProviderRequest, ex, stopwatch.ElapsedMilliseconds);
            }

            throw;
        }
    }

    private async Task<HttpProviderRequest<dynamic>?> CreateHttpProviderRequestFromHttpRequestMessage(HttpRequestMessage request)
    {
        try
        {
            var body = string.Empty;
            if (request.Content != null)
            {
                body = await request.Content.ReadAsStringAsync();
            }

            return new HttpProviderRequest<dynamic>
            {
                Uri = request.RequestUri?.ToString() ?? "",
                BaseAddress = request.RequestUri?.GetLeftPart(UriPartial.Authority) ?? "",
                Body = body,
                // Note: We don't have access to Provider and Service enums here
                // These will be set by the actual HttpProvider methods
            };
        }
        catch
        {
            return null;
        }
    }
}
