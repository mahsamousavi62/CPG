using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Policies;

/// <summary>
/// Wrapper to log SOAP requests and responses including timeout scenarios
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
        Func<Task<TResponse>> soapCall);
}

public class SoapLoggingWrapper : ISoapLoggingWrapper
{
    private readonly ILogger<SoapLoggingWrapper> _logger;

    public SoapLoggingWrapper(ILogger<SoapLoggingWrapper> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> ExecuteWithLoggingAsync<TRequest, TResponse>(
        string serviceName,
        string operationName,
        TRequest request,
        Func<Task<TResponse>> soapCall)
    {
        var requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
        var stopwatch = Stopwatch.StartNew();

        // Serialize request
        var requestBody = SerializeObject(request);

        _logger.LogInformation(
            "[{RequestId}] SOAP Request: {ServiceName}.{OperationName} | Request: {RequestBody}",
            requestId,
            serviceName,
            operationName,
            TruncateIfNeeded(requestBody, 2000));

        TResponse response = default;

        try
        {
            response = await soapCall();
            stopwatch.Stop();

            var responseBody = SerializeObject(response);

            _logger.LogInformation(
                "[{RequestId}] SOAP Response: {ServiceName}.{OperationName} | Duration: {DurationMs}ms | Response: {ResponseBody}",
                requestId,
                serviceName,
                operationName,
                stopwatch.ElapsedMilliseconds,
                TruncateIfNeeded(responseBody, 2000));

            return response;
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] SOAP TIMEOUT: {ServiceName}.{OperationName} after {DurationMs}ms | " +
                "Request: {RequestBody} | Exception: {ExceptionMessage}",
                requestId,
                serviceName,
                operationName,
                stopwatch.ElapsedMilliseconds,
                TruncateIfNeeded(requestBody, 3000),
                ex.Message);

            throw;
        }
        catch (TimeoutException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] SOAP TIMEOUT: {ServiceName}.{OperationName} after {DurationMs}ms | " +
                "Request: {RequestBody} | Exception: {ExceptionMessage}",
                requestId,
                serviceName,
                operationName,
                stopwatch.ElapsedMilliseconds,
                TruncateIfNeeded(requestBody, 3000),
                ex.Message);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] SOAP FAILED: {ServiceName}.{OperationName} after {DurationMs}ms | " +
                "Request: {RequestBody} | Exception: {ExceptionType} - {ExceptionMessage}",
                requestId,
                serviceName,
                operationName,
                stopwatch.ElapsedMilliseconds,
                TruncateIfNeeded(requestBody, 3000),
                ex.GetType().Name,
                ex.Message);

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
