using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Logging;

// کلاس ساده برای لاگ SOAP - بدون interface پیچیده
public class SoapLogger
{
    private readonly ILogger<SoapLogger> _logger;
    private readonly ILogService _logService;

    public SoapLogger(ILogger<SoapLogger> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    public async Task<TResponse> LogSoapCallAsync<TRequest, TResponse>(
        string serviceName,
        string operationName,
        TRequest request,
        Func<Task<TResponse>> soapCall)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestJson = JsonConvert.SerializeObject(request);
        string responseJson = null;

        try
        {
            var response = await soapCall();
            stopwatch.Stop();

            responseJson = JsonConvert.SerializeObject(response);

            // لاگ موفق
            _logger.LogInformation(
                "[SOAP] {ServiceName}.{OperationName} completed in {DurationMs}ms",
                serviceName, operationName, stopwatch.ElapsedMilliseconds);

            // استفاده از متد موجود AddServiceCallLog
            _logService.ServiceName = serviceName;
            _logService.ServiceType = Enums.ServiceType.External;
            _logService.ProviderTypeInLog = DetermineProviderType(serviceName);

            _logService.AddServiceCallLog(
                TruncateBody(requestJson, 3000),
                TruncateBody(responseJson, 2000),
                0, // موفق
                string.Empty);

            return response;
        }
        catch (TimeoutException ex)
        {
            stopwatch.Stop();

            // LogTimeout method will be added to LogService in S004
            _logger.LogError(ex,
                "[SOAP TIMEOUT] {ServiceName}.{OperationName} after {DurationMs}ms | Request: {RequestBody}",
                serviceName, operationName, stopwatch.ElapsedMilliseconds,
                TruncateBody(requestJson, 3000));

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "[SOAP] {ServiceName}.{OperationName} FAILED after {DurationMs}ms | Request: {RequestBody}",
                serviceName, operationName, stopwatch.ElapsedMilliseconds,
                TruncateBody(requestJson, 3000));

            throw;
        }
    }

    private Enums.ProviderTypeInLog DetermineProviderType(string serviceName)
    {
        return serviceName switch
        {
            "BehPardakht" => Enums.ProviderTypeInLog.Pec,
            "PEC" => Enums.ProviderTypeInLog.Pec,
            "AsanPardakht" => Enums.ProviderTypeInLog.AsanPardakht,
            _ => Enums.ProviderTypeInLog.AsanPardakht
        };
    }

    private static string TruncateBody(string body, int maxLength)
    {
        if (string.IsNullOrEmpty(body) || body.Length <= maxLength)
            return body;

        return body.Substring(0, maxLength) + $"... [بریده: {body.Length - maxLength} کاراکتر]";
    }
}
