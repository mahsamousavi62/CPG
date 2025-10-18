# Technical Architecture: Resilience and Observability Enhancement

**Feature Branch**: `feature/002-resilience-observability-enhancement`
**Created**: 2025-10-18
**Status**: Final
**Complexity**: 🟠 **MEDIUM** (6-8 روز کاری)

---

## معماری کلی

این سند معماری فنی برای افزایش قابلیت ارتجاع (Resilience) و مشاهده‌پذیری (Observability) در سیستم درگاه پرداخت CPG را تعریف می‌کند. راه‌حل شامل پیاده‌سازی Retry با Polly، لاگ‌گیری جامع Request/Response، استانداردسازی لاگ‌ها، و لاگ‌گیری سرویس‌های زیرساختی (Database, MinIO) است.

```
┌────────────────────────────────────────────────────────┐
│                   CPG.API                              │
│  - استفاده از HttpContext.TraceIdentifier            │
└────────────────┬───────────────────────────────────────┘
                 │
┌────────────────▼───────────────────────────────────────┐
│          CPG.Infrastructure                            │
│                                                        │
│  Policies/ (جدید)                                     │
│  ├─ PollyExtensions.cs (متدهای Extension)            │
│  └─ PollyLoggingHandler.cs (DelegatingHandler)        │
│                                                        │
│  Logging/ (توسعه)                                     │
│  ├─ LogService.cs (متدهای جدید)                      │
│  └─ SoapLogger.cs (wrapper ساده برای SOAP)           │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│      CPG.Infrastructure.Persistence                    │
│                                                        │
│  Interceptors/                                         │
│  └─ DatabaseLoggingInterceptor.cs (جدید)              │
└────────────────────────────────────────────────────────┘
```

---

## 1. Polly Retry Infrastructure

### 1.1 PollyExtensions - متدهای Extension ساده

**مسیر**: `src/CPG.Infrastructure/Policies/PollyExtensions.cs`

```csharp
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace CPG.Infrastructure.Policies;

public static class PollyExtensions
{
    // برای استفاده در HttpClient
    public static IHttpClientBuilder AddStandardRetryPolicy(
        this IHttpClientBuilder builder,
        int maxRetryAttempts = 3)
    {
        return builder.AddStandardResilienceHandler(options =>
        {
            // فقط Retry - بدون CircuitBreaker
            options.Retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = maxRetryAttempts,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutException>()
                    .HandleResult(response =>
                        response.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                        response.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                        response.StatusCode == (System.Net.HttpStatusCode)429),
                OnRetry = args =>
                {
                    var logger = args.Context.ServiceProvider?.GetService<ILogger<PollyExtensions>>();
                    logger?.LogWarning("[Polly Retry] Attempt {AttemptNumber} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            };

            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        });
    }

    // برای استفاده دستی در SOAP یا کدهای دیگر
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        ILogger logger,
        int maxRetryAttempts = 3,
        string serviceName = null)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<System.ServiceModel.CommunicationException>()
            .WaitAndRetryAsync(
                maxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogWarning(exception,
                        "[Polly Retry] {ServiceName} - Retry {RetryCount}/{MaxRetryAttempts} after {Delay}ms",
                        serviceName, retryCount, maxRetryAttempts, timeSpan.TotalMilliseconds);
                });

        return await retryPolicy.ExecuteAsync(operation);
    }
}
```

**استفاده آسان در DependencyInjection.cs**:
```csharp
// افزودن به HttpClient های موجود
services.AddHttpClient("charisPayClient", c => { ... })
    .AddStandardRetryPolicy(maxRetryAttempts: 3)
    .AddHttpMessageHandler<PollyLoggingHandler>();

services.AddHttpClient("asanpardakhtClient", c => { ... })
    .AddStandardRetryPolicy(maxRetryAttempts: 5)  // AsanPardakht needs 5 retries
    .AddHttpMessageHandler<PollyLoggingHandler>();
```

---

### 1.2 PollyLoggingHandler - لاگ Request/Response در Timeout

**مسیر**: `src/CPG.Infrastructure/Policies/PollyLoggingHandler.cs`

```csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CPG.Domain.SharedKernel.Logging;

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
```

---

## 2. LogService Extensions - توسعه کلاس موجود

**مسیر**: `src/CPG.Infrastructure/Logging/LogService.cs` (تغییر در کلاس موجود)

```csharp
// متدهای جدید برای اضافه کردن به کلاس LogService موجود
public partial class LogService
{
    // متد جدید: لاگ timeout با جزئیات کامل
    public void LogTimeout(string serviceName, string operationName, TimeSpan duration,
        string requestBody, string partialResponse)
    {
        var timeoutLog = new
        {
            ServiceName = serviceName,
            OperationName = operationName,
            DurationMs = (long)duration.TotalMilliseconds,
            RequestBody = TruncateBody(MaskSensitiveData(requestBody), 3000),
            PartialResponseBody = TruncateBody(MaskSensitiveData(partialResponse), 2000),
            Timestamp = DateTime.UtcNow,
            RequestId = GetCorrelationId()
        };

        using (LogContext.PushProperty("Timeout", timeoutLog, true))
        {
            _logger.LogError(
                "[TIMEOUT] {ServiceName}.{OperationName} after {DurationMs}ms | RequestId: {RequestId}",
                serviceName, operationName, timeoutLog.DurationMs, timeoutLog.RequestId);
        }
    }

    // متد جدید: برش بدنه برای جلوگیری از لاگ زیاد
    public string TruncateBody(string body, int maxLength)
    {
        if (string.IsNullOrEmpty(body) || body.Length <= maxLength)
            return body;

        return body.Substring(0, maxLength) + $"... [بریده: {body.Length - maxLength} کاراکتر]";
    }

    // متد جدید: گرفتن Correlation ID از HttpContext
    private string GetCorrelationId()
    {
        return _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
    }

    // توسعه متد موجود: Masking حساس‌تر
    private string MaskSensitiveData(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // استفاده از regex موجود
        content = MyRegex().Replace(content, Constants.Replaceformat);

        // اضافه کردن masking برای PAN (نگه داشتن 4 رقم آخر)
        content = System.Text.RegularExpressions.Regex.Replace(
            content, @"\b(\d{12})(\d{4})\b", "************$2");

        // CVV2 کامل پاک شود
        content = System.Text.RegularExpressions.Regex.Replace(
            content, @"""cvv2?""\s*:\s*""\d{3,4}""", "\"cvv2\":\"***\"",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Token ها رو به 10 کاراکتر اول برش بزن
        content = System.Text.RegularExpressions.Regex.Replace(
            content, @"""(token|access_token)""\s*:\s*""([^""]{10})[^""]*""",
            "\"$1\":\"$2...\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return content;
    }
}
```

---

## 3. SOAP Logging - Wrapper ساده

**مسیر**: `src/CPG.Infrastructure/Logging/SoapLogger.cs` (جدید)

```csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CPG.Domain.SharedKernel.Logging;

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
                _logService.TruncateBody(requestJson, 3000),
                _logService.TruncateBody(responseJson, 2000),
                0, // موفق
                string.Empty);

            return response;
        }
        catch (TimeoutException ex)
        {
            stopwatch.Stop();

            _logService.LogTimeout(serviceName, operationName, stopwatch.Elapsed,
                requestJson, responseJson);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "[SOAP] {ServiceName}.{OperationName} FAILED after {DurationMs}ms | Request: {RequestBody}",
                serviceName, operationName, stopwatch.ElapsedMilliseconds,
                _logService.TruncateBody(requestJson, 3000));

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
}
```

---

## 4. Database Logging Interceptor

**مسیر**: `src/CPG.Infrastructure.Persistence/Interceptors/DatabaseLoggingInterceptor.cs`

```csharp
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace CPG.Infrastructure.Persistence.Interceptors;

public class DatabaseLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<DatabaseLoggingInterceptor> _logger;
    private readonly int _slowQueryThresholdMs;
    private readonly Dictionary<DbCommand, QueryContext> _queryContexts = new();

    public DatabaseLoggingInterceptor(
        ILogger<DatabaseLoggingInterceptor> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _slowQueryThresholdMs = configuration.GetValue<int>("Infrastructure:Database:Logging:SlowQueryThresholdMs", 5000);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        _queryContexts[command] = new QueryContext
        {
            StartTime = DateTime.UtcNow,
            ContextType = eventData.Context?.GetType().Name  // "ReadDbContext" or "WriteDbContext"
        };

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (_queryContexts.TryGetValue(command, out var context))
        {
            var duration = eventData.Duration.TotalMilliseconds;

            // لاگ همه query ها در سطح Information
            _logger.LogInformation(
                "[Database] {ContextType} Query executed in {DurationMs}ms | Rows: {RowsAffected}",
                context.ContextType, duration, result.RecordsAffected);

            // تشخیص Slow Query
            if (duration > _slowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "[Database] SLOW QUERY detected ({DurationMs}ms) | Context: {ContextType} | Query: {QueryText}",
                    duration, context.ContextType, command.CommandText);
            }

            _queryContexts.Remove(command);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var duration = eventData.Duration.TotalMilliseconds;

        _logger.LogInformation(
            "[Database] NonQuery executed in {DurationMs}ms | Context: {ContextType} | Rows: {RowsAffected}",
            duration, eventData.Context?.GetType().Name, result);

        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    private class QueryContext
    {
        public DateTime StartTime { get; set; }
        public string ContextType { get; set; }
    }
}
```

**Registration در Persistence DI**:
```csharp
// در فایل DependencyInjection.cs پروژه Persistence
services.AddSingleton<DatabaseLoggingInterceptor>();

services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(connectionString)
           .AddInterceptors(
               serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
               serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>(),
               serviceProvider.GetRequiredService<DatabaseLoggingInterceptor>());  // جدید
});
```

---

## 5. MinIO Logging - توسعه کلاس موجود

**مسیر**: `src/CPG.Infrastructure/Minio/MinioProvider.cs` (تغییر متدهای موجود)

```csharp
// تغییرات در متد PutObject موجود
public async Task<string> PutObject(string uploadFromEntityType, IFile file)
{
    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

    var bucketName = _configuration["Infrastructure:Minio:bucketName"];
    var sanitizedFileName = Path.GetFileName(file.FileName);
    var objectName = $"{uploadFromEntityType}/{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{sanitizedFileName}";

    await file.ReadFile();
    file.Content.Seek(0, SeekOrigin.Begin);

    var stopwatch = Stopwatch.StartNew();  // اضافه شد
    try
    {
        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithContentType(file.ContentType)
            .WithObjectSize(file.Length)
            .WithStreamData(file.Content);

        var response = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        stopwatch.Stop();  // اضافه شد

        // لاگ ساختاریافته - به جای LogWarning
        _logger.LogInformation(
            "[MinIO] PutObject succeeded in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
            stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);

        return response.ObjectName;
    }
    catch (MinioException exc)
    {
        stopwatch.Stop();  // اضافه شد

        _logger.LogError(exc,
            "[MinIO] PutObject FAILED after {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
            stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);

        return exc.Message;
    }
}
```

---

## 6. Provider Integration - نحوه استفاده

### 6.1 BehPardakhtProvider - استفاده از Retry و SOAP Logger

**مسیر**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs`

```csharp
// اضافه کردن فیلدها به کلاس
private readonly SoapLogger _soapLogger;

public BehPardakhtProvider(
    ReadDbContext context,
    IApplicationSettingsRepository applicationSettingsRepository,
    ILogService logService,
    ILogger<BehPardakhtProvider> logger,
    SoapLogger soapLogger)  // اضافه شد
{
    _applicationSettingRepositoy = applicationSettingsRepository;
    _logService = logService;
    _logger = logger;
    context = context;
    _soapLogger = soapLogger;  // اضافه شد
}

// تغییر در متد GetPaymentTokenAsync
public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
{
    GetDataFromJsonProvider(request.ProviderData);
    var configViewModel = await _applicationSettingRepositoy.GetAllApplicationSettings();
    var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
    string callBackUrl = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress, trackerId.ToString(), configViewModel.CPG_BackEnd);

    var payRequest = new bpPayRequest
    {
        Body = new bpPayRequestBody
        {
            terminalId = terminalId,
            userName = userName,
            userPassword = password,
            orderId = long.Parse(trackerId),
            amount = (long)request.PaymentRequestAmount,
            localDate = DateTime.Now.ToString("yyyyMMdd"),
            localTime = DateTime.Now.ToString("HHmmss"),
            callBackUrl = callBackUrl,
            payerId = "0",
            mobileNo = !string.IsNullOrEmpty(request.MobileNumber) ? $"98{request.MobileNumber.Remove(0, 1)}" : null,
            encPan = null,
            panHiddenMode = null,
            enc = request.NationalCodeMatchingRequied ? CreateAdditionalData(request.NationalCode, request.ShaparakKey, request.ShaparakIv, request.ThirdPartyCode) : string.Empty,
            cartItem = null,
            additionalData = null,
        }
    };

    // استفاده از PollyExtensions برای Retry + SoapLogger برای لاگ
    return await PollyExtensions.ExecuteWithRetryAsync(async () =>
    {
        return await _soapLogger.LogSoapCallAsync(
            serviceName: "BehPardakht",
            operationName: "GetPaymentToken",
            request: payRequest,
            soapCall: async () =>
            {
                using (var client = new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort))
                {
                    var response = await client.bpPayRequestAsync(
                        payRequest.Body.terminalId, payRequest.Body.userName, payRequest.Body.userPassword,
                        payRequest.Body.orderId, payRequest.Body.amount, payRequest.Body.localDate, payRequest.Body.localTime,
                        payRequest.Body.additionalData, payRequest.Body.callBackUrl, payRequest.Body.payerId,
                        payRequest.Body.mobileNo, payRequest.Body.encPan, payRequest.Body.panHiddenMode,
                        payRequest.Body.cartItem, payRequest.Body.enc);

                    var responseData = response.Body.@return.Split(',');
                    short status = short.TryParse(responseData[0], out short value) ? value : (short)1;
                    string token = string.Empty;
                    if (responseData.Length > 1)
                    {
                        token = responseData[1];
                    }

                    return new PaymentTokenResponse
                    {
                        Token = token,
                        StatusCode = status == 0 ? (short)HttpStatusCode.OK : status,
                        IpgBaseUrl = request.IpgBaseUrl,
                        TrackerId = trackerId.ToString(),
                    };
                }
            });
    }, _logger, maxRetryAttempts: 3, serviceName: "BehPardakht");

    // حذف کامل try-catch و manual retry (tokenFailCounter, etc.)
}
```

---

## 7. Configuration - appsettings.json

```json
{
  "Infrastructure": {
    "Database": {
      "Logging": {
        "SlowQueryThresholdMs": 5000
      }
    }
  }
}
```

**توجه**: تنظیمات Retry مستقیماً در کد هستند (نه config file) برای سادگی - همانطور که پروژه فعلی است.

---

## 8. Dependency Injection - تغییرات

**مسیر**: `src/CPG.Infrastructure/DependencyInjection.cs`

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing registrations ...

    // جدید: PollyLoggingHandler
    services.AddScoped<PollyLoggingHandler>();

    // جدید: SoapLogger
    services.AddScoped<SoapLogger>();

    // تغییر: اضافه کردن Retry Policy به HttpClient های موجود
    services.AddHttpClient("charisPayClient", c =>
    {
        c.BaseAddress = new Uri(charisPayConfig.BaseUrl);
        c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddStandardRetryPolicy(maxRetryAttempts: 3)  // اضافه شد
    .AddHttpMessageHandler<PollyLoggingHandler>();  // اضافه شد

    services.AddHttpClient("idpClient", c =>
    {
        c.BaseAddress = new Uri(jwtConfig.Authority);
        c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddStandardRetryPolicy(maxRetryAttempts: 3)
    .AddHttpMessageHandler<PollyLoggingHandler>();

    services.AddHttpClient("neoBankClient", c =>
    {
        c.BaseAddress = new Uri(neoBankConfig.BaseUrl);
        c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddStandardRetryPolicy(maxRetryAttempts: 3)
    .AddHttpMessageHandler<PollyLoggingHandler>();

    services.AddHttpClient("asanpardakhtClient", c =>
    {
        c.BaseAddress = new Uri("https://ipgrest.asanpardakht.ir/");
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
    })
    .AddStandardRetryPolicy(maxRetryAttempts: 5)  // AsanPardakht needs 5 retries
    .AddHttpMessageHandler<PollyLoggingHandler>();

    services.AddHttpClient("charismaCardClient", c =>
    {
        c.BaseAddress = new Uri(charismaCardConfig.BaseUrl);
        c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddStandardRetryPolicy(maxRetryAttempts: 3)
    .AddHttpMessageHandler<PollyLoggingHandler>();

    return services;
}
```

---

## 9. Migration Plan - برنامه پیاده‌سازی

### فاز 1: زیرساخت (2 روز)
1. ایجاد `PollyExtensions.cs`
2. ایجاد `PollyLoggingHandler.cs`
3. ایجاد `SoapLogger.cs`
4. توسعه `LogService.cs` (متدهای جدید)
5. رجیستر در DI

**خروجی**: زیرساخت Polly و Logging آماده

### فاز 2: HTTP Client Integration (1-2 روز)
1. اضافه کردن `.AddStandardRetryPolicy()` به تمام HttpClient ها در DI
2. اضافه کردن `.AddHttpMessageHandler<PollyLoggingHandler>()` به تمام HttpClient ها
3. تست با AsanPardakhtProvider
4. بررسی لاگ‌ها در Elasticsearch

**خروجی**: تمام HTTP Client ها retry و logging دارند

### فاز 3: SOAP Provider Integration (2 روز)
1. اضافه کردن `SoapLogger` به constructor های BehPardakhtProvider و PecProvider
2. Wrap کردن تمام SOAP call ها با `PollyExtensions.ExecuteWithRetryAsync()` و `SoapLogger.LogSoapCallAsync()`
3. **حذف** تمام manual retry logic (tokenFailCounter, verifyFailCounter, settleFailCounter)
4. تست با BehPardakht و PEC
5. بررسی لاگ‌های SOAP در Elasticsearch

**خروجی**: SOAP provider ها retry و logging مرکزی دارند

### فاز 4: Database & MinIO Logging (1-2 روز)
1. ایجاد `DatabaseLoggingInterceptor`
2. رجیستر در `WriteDbContext` و `ReadDbContext`
3. توسعه `MinioProvider` (اضافه کردن Stopwatch و لاگ‌های بهتر)
4. تست query logging و slow query detection
5. تست MinIO logging

**خروجی**: Database و MinIO logging کامل

### فاز 5: تست و Validation (1 روز)
1. تست performance (بررسی overhead لاگ‌ها)
2. تست Sensitive Data Masking (PAN, CVV2, Token)
3. تست Retry تحت فشار
4. بررسی کلی لاگ‌ها در Elasticsearch

**خروجی**: سیستم آماده Production

---

## 10. نکات مهم پیاده‌سازی

### ✅ کارهایی که **باید** انجام شوند:
1. حذف **کامل** manual retry logic از Provider ها (tokenFailCounter, verifyFailCounter, etc.)
2. اضافه کردن `.AddStandardRetryPolicy()` به **همه** HttpClient های موجود
3. استفاده از `HttpContext.TraceIdentifier` به عنوان Correlation ID (نیاز به middleware جدید نیست)
4. Masking داده‌های حساس (PAN, CVV2, Token) در تمام لاگ‌ها
5. Truncate کردن Request/Response body (3000/2000 کاراکتر)

### ❌ کارهایی که **نباید** انجام شوند:
1. اضافه کردن Circuit Breaker (طبق درخواست کاربر)
2. ایجاد class/interface پیچیده برای Polly (فقط Extension methods)
3. ایجاد Model های جداگانه برای لاگ (استفاده از anonymous objects و CallLogModel موجود)
4. تغییر دادن ساختار کلی پروژه

### 🎯 سبک کدنویسی:
- شبیه به کد موجود در پروژه (ساده و مستقیم)
- استفاده از متدهای Extension برای Polly
- استفاده از کلاس‌های موجود (LogService, CallLogModel) تا حد امکان
- لاگ با Serilog و `LogContext.PushProperty` (مانند کد موجود)

---

## 11. Risk Mitigation - کاهش ریسک

### Risk 1: Double Retry (Polly + Manual Retry)
**راه حل**: حذف **کامل** manual retry در Provider ها هنگام اضافه کردن Polly

### Risk 2: PCI-DSS Compliance (لاگ داده‌های حساس)
**راه حل**: تست کامل Masking با داده‌های واقعی قبل از Production

### Risk 3: Performance Overhead
**راه حل**: لاگ کامل Request/Response **فقط** در Timeout یا Error - نه Success

---

**وضعیت معماری**: ✅ **آماده پیاده‌سازی**
**پیچیدگی**: 🟠 **MEDIUM** (6-8 روز کاری)
**مرحله بعد**: شروع Phase 1 - ایجاد کلاس‌های زیرساختی
