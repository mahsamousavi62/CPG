# Technical Research: Resilience and Observability Enhancement

**Feature Branch**: `feature/002-resilience-observability-enhancement`
**Research Date**: 2025-10-18
**Status**: Complete

## Executive Summary

This research document analyzes the current CPG payment gateway infrastructure to identify technical requirements and implementation strategies for enhancing system resilience and observability. The analysis covers retry policies, logging infrastructure, SOAP/HTTP service integration, and database/file storage logging capabilities.

---

## Current State Analysis

### 1. Retry Policy Infrastructure

#### Current Implementation
**Status**: ⚠️ **Partial - Ad-hoc retry in providers only**

**Findings**:
- **NO centralized Polly infrastructure** despite Polly 8.x being referenced in DependencyInjection.cs:22
- **Manual retry counters** implemented in each provider class:
  - `AsanPardakhtProvider.cs:28` - `serviceCallMaxTryCounter = 5` (hardcoded)
  - `BehPardakhtProvider.cs:28` - `serviceCallMaxTryCounter = 5` (hardcoded)
  - Each operation (token, verify, settle) has separate fail counters (tokenFailCounter, verifyFailCounter, settleFailCounter)

**Current Retry Pattern**:
```csharp
// AsanPardakhtProvider.cs:186-192
private async Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(...)
{
    if (tokenFailCounter < serviceCallMaxTryCounter)  // Simple counter check
    {
        tokenFailCounter++;                           // No backoff delay
        return await GetPaymentTokenAsync(baseRequest) as TResponse;
    }
    return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
}
```

**Problems Identified**:
1. ❌ No exponential backoff - immediate retries
2. ❌ No jitter - synchronized retry storms possible
3. ❌ No circuit breaker - continuous retries when service is down
4. ❌ Inconsistent retry counts across providers (AsanPardakht=5, manual in code)
5. ❌ No retry for HTTP clients (HttpProvider, NeoBankProvider, CharisPayProvider)
6. ❌ Retry logic embedded in error handlers - not reusable

**HTTP Client Configuration**:
- `DependencyInjection.cs:119-161` registers 5 named HttpClients:
  - `charisPayClient`, `idpClient`, `neoBankClient`, `asanpardakhtClient`, `charismaCardClient`
- **NO Polly policies attached** to any HttpClient
- Default HttpClient (via `CreateClient()` in HttpProvider) also **lacks policies**

**SOAP Service Calls**:
- `BehPardakhtProvider.cs:61-125` - PaymentGatewayClient directly instantiated
- Try-catch with manual retry in catch block
- No timeout configuration beyond default

---

### 2. Request/Response Logging Infrastructure

#### HTTP Logging (REST APIs)

**Current Implementation**: ✅ **Basic logging exists**

**LogService.cs Analysis**:
- Located: `src/CPG.Infrastructure/Logging/LogService.cs`
- Current features:
  - `AddServiceCallLog<TBody>(HttpProviderRequest<TBody>, HttpResponseMessage, string resString)` (line 28-61)
  - Logs request/response body via Serilog with `LogContext.PushProperty`
  - **Sensitive data masking**: Uses regex pattern `MyRegex()` (line 32, 37)
  - Logs to Elasticsearch via structured logging: `_logger.LogInformation("[CallLog] {@CallLog}", callLog)` (line 59)

**CallLogModel Structure** (line 41-55):
```csharp
var callLog = new CallLogModel
{
    RequestBody = reqString,          // ✅ Full body logged
    ResponseBody = resString,         // ✅ Full body logged
    ServiceCallDate = DateTime.Now,
    ServiceCallUrl = request.Uri,
    ServiceCallStatus = response.StatusCode == HttpStatusCode.OK,
    ServiceType = request.Service,    // Enum - needs better categorization
    CreationDate = DateTime.Now,
    CreationUserId = UserId == 0 ? 1 : UserId,
    ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
    ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
    ProviderType = request.Provider,  // Enum - provider identification
    AuditType = Enums.AuditType.Provider
};
```

**Gaps Identified**:
1. ❌ **No timeout-specific logging** - timeout scenarios not distinguished from other failures
2. ❌ **No request/response duration tracking** - performance metrics missing
3. ❌ **No correlation ID propagation** - distributed tracing not implemented
4. ❌ **No truncation strategy** - large bodies could cause storage issues
5. ❌ **Missing fields**: OperationName, RequestId, structured Duration (ms)

#### SOAP Logging

**Current Implementation**: ⚠️ **Minimal SOAP logging**

**BehPardakhtProvider Logging** (line 243-251):
```csharp
private void CreateLog<T1, T2>(T1 request, T2 response, string serviceName, short status, string message, Enums.ServiceType serviceType)
{
    _logService.ServiceName = serviceName;  // ✅ Operation name captured
    _logService.ServiceType = serviceType;   // Enum - but not standardized
    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;

    _logService.AddServiceCallLog(JsonConvert.SerializeObject(request),
        JsonConvert.SerializeObject(response), status, message);
}
```

**Issues**:
1. ❌ **No SOAP envelope logging** - only serialized objects, not actual XML
2. ❌ **No timeout handling** - exceptions not logged with full context
3. ❌ **Inconsistent usage** - not all SOAP operations log (e.g., PecProvider not analyzed)

---

### 3. Log Categorization and Standardization

#### Current Categorization System

**ServiceType Enum** (referenced but not defined in analyzed files):
- Used in `LogService.cs:26` as property
- Used in `CallLogModel:48` as `request.Service`
- Examples from code: `ServiceType.AsanPardakhtToken`, `ServiceType.BehPardakhtVerify`

**ProviderTypeInLog Enum**:
- Examples: `Enums.ProviderTypeInLog.AsanPardakht`, `Enums.ProviderTypeInLog.Pec`

**Problems**:
1. ❌ **No Internal/External distinction** - all logs treated as provider logs
2. ❌ **No OperationName field** - operation buried in ServiceType enum
3. ❌ **No RequestId/Correlation ID** - cannot trace requests across layers
4. ❌ **No Duration in milliseconds** - performance analysis difficult
5. ❌ **No Status field** (beyond success/fail) - need numeric status codes
6. ❌ **Not aligned with task 18535 format** (referenced in spec but not found in codebase)

**Desired Structure** (from FR-010):
```
- ServiceName: string (e.g., "AsanPardakht", "PaymentRequestHandler")
- ServiceType: enum (Internal/External)
- OperationName: string (e.g., "GetToken", "VerifyTransaction")
- RequestId: GUID (correlation ID)
- Duration: long (milliseconds)
- Status: int (HTTP status code or operation status)
```

---

### 4. Infrastructure Service Logging

#### MinIO Logging

**Current State**: ⚠️ **Basic error logging only**

**MinioProvider.cs Analysis**:
- `PutObject` method (line 48-82):
  - ✅ Logs response on success: `_logger.LogWarning($"Response Minio : {resString}")` (line 73)
  - ✅ Logs exceptions: `_logger.LogError(exc, ...)` (line 79)
  - ❌ **No structured logging** - uses string interpolation
  - ❌ **Missing metadata**: bucket name, object key, file size, duration not logged
  - ❌ **No operation type categorization** (upload/download/delete)

**Gaps**:
- `GetObjectByName` (line 84-128): Only error logging, no success metrics
- `PresignedGetObject` (line 130-167): Only error logging
- No duration tracking for any operation
- No S3-compatible operation metadata logged

#### Entity Framework Core Logging

**Current State**: ❌ **NO custom database logging interceptor**

**Found Interceptors**:
1. **AuditableEntityInterceptor.cs**: Handles CreationUserId/ModificationUserId (line 13-58)
2. **DispatchDomainEventsInterceptor.cs**: Handles domain event dispatch

**Missing**:
- ❌ No `DatabaseLoggingInterceptor` (referenced in spec but not found)
- ❌ No query text logging
- ❌ No execution time tracking
- ❌ No slow query detection
- ❌ No N+1 query pattern detection
- ❌ No Read/Write context differentiation in logs

**DbContext Configuration** (WriteDbContext.cs:21-23):
```csharp
public class WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : DbContext(options)
```
- Uses constructor injection for options
- Interceptors likely configured in `DependencyInjection` (not analyzed in detail)

---

## Technology Stack Assessment

### Polly Resilience Library

**Version**: Polly 8.x (referenced in DependencyInjection.cs:22)
**Documentation**: https://www.pollydocs.org/

**Recommended Patterns** (based on v8.x):
1. **Resilience Pipeline**: New v8.x API replacing `Policy` classes
2. **Retry Strategy**: `ResiliencePipelineBuilder<T>.AddRetry()`
   - Exponential backoff with jitter built-in
   - Configurable via `RetryStrategyOptions`
3. **Circuit Breaker**: `AddCircuitBreaker()`
   - State persistence via custom storage
4. **Timeout**: `AddTimeout()`
   - Per-operation timeout configuration

**Integration Points**:
- HttpClient: `IHttpClientBuilder.AddResilienceHandler()`
- SOAP clients: Manual wrapping with `ResiliencePipeline<T>.ExecuteAsync()`

**Configuration Pattern** (recommended):
```csharp
// appsettings.json
"Infrastructure": {
  "Resilience": {
    "Retry": {
      "MaxRetryAttempts": 3,
      "BaseDelaySeconds": 2,
      "UseJitter": true
    },
    "CircuitBreaker": {
      "FailureThreshold": 5,
      "SamplingDurationSeconds": 30,
      "DurationOfBreakSeconds": 60
    },
    "Timeout": {
      "TimeoutSeconds": 30,
      "SoapTimeoutSeconds": 45
    }
  }
}
```

---

### Serilog Logging Infrastructure

**Current Configuration** (not directly analyzed, inferred from usage):
- Serilog with Elasticsearch sink (based on Context.md)
- Structured logging via `LogContext.PushProperty`
- File and Console sinks also configured

**Enhancement Opportunities**:
1. **Enrichers**: Add correlation ID, operation name enrichers
2. **Sub-loggers**: Create categorized loggers per service type
3. **Minimum level overrides**: Configure different levels for Internal vs External
4. **Async logging**: Ensure async sinks for high-volume scenarios

**Sensitive Data Masking**:
- Current: Regex-based in LogService (Constants.Pattern, Constants.Replaceformat)
- Needs: Extension to handle PCI-DSS patterns (PAN, CVV2, tokens)

---

### Entity Framework Core 8.0

**Interceptor Capabilities**:
- `IDbCommandInterceptor`: Query command interception
  - `CommandExecuting` / `CommandExecuted` events
  - Access to query text, parameters, duration
- `ISaveChangesInterceptor`: Already used (Auditable, DomainEvents)

**Recommended Interceptor**: `DatabaseLoggingInterceptor : DbCommandInterceptor`
```csharp
public override ValueTask<DbDataReader> ReaderExecutedAsync(
    DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken)
{
    var duration = eventData.Duration.TotalMilliseconds;
    var queryText = command.CommandText;
    var contextType = eventData.Context?.GetType().Name; // "ReadDbContext" or "WriteDbContext"

    // Log with structured format
    _logger.LogInformation("[Database] Query executed in {Duration}ms | Context: {ContextType} | Rows: {RowsAffected}",
        duration, contextType, result.RecordsAffected);

    // Slow query detection
    if (duration > _slowQueryThreshold)
    {
        _logger.LogWarning("[Database] SLOW QUERY detected ({Duration}ms) | Query: {QueryText}", duration, queryText);
    }

    return new ValueTask<DbDataReader>(result);
}
```

---

### MinIO (.NET SDK)

**Current Version**: Minio.AspNetCore (referenced in DependencyInjection.cs:19, 71)

**Logging Strategy**:
- Wrap all MinIO operations in try-catch with structured logging
- Use `PutObjectResponse`, `GetObjectResponse` metadata for logging
- Track operation start/end times with `Stopwatch` or similar

---

## Implementation Recommendations

### Phase 1: Polly Resilience Infrastructure

**Priority**: 🔴 **CRITICAL**

**New Components**:
1. **Service**: `IPollyPolicyService` + `PollyPolicyService`
   - Location: `src/CPG.Infrastructure/Policies/`
   - Methods:
     - `ExecuteAsync<T>(Func<Task<T>> action, string serviceName)`
     - `GetHttpClientPolicy()` for HttpClient builder
     - `GetSoapClientPolicy()` for SOAP wrapping

2. **Configuration**: `PolicyConfig.cs`
   - Maps to `appsettings.json:Infrastructure:Resilience`
   - Per-provider policy overrides support

3. **HTTP Logging Handler**: `PollyLoggingHandler : DelegatingHandler`
   - Logs request/response on timeout
   - Automatically attached to all named HttpClients

**Integration Points**:
- `DependencyInjection.cs:119-161`: Modify HttpClient registrations
  ```csharp
  services.AddHttpClient("charisPayClient", c => { ... })
      .AddResilienceHandler("charisPayPolicy", (builder, context) => {
          var pollyService = context.ServiceProvider.GetRequiredService<IPollyPolicyService>();
          builder.AddRetry(pollyService.GetRetryOptions())
                 .AddCircuitBreaker(pollyService.GetCircuitBreakerOptions())
                 .AddTimeout(pollyService.GetTimeoutOptions());
      })
      .AddHttpMessageHandler<PollyLoggingHandler>();
  ```

- Providers: Wrap SOAP calls
  ```csharp
  // BehPardakhtProvider.cs
  return await _pollyPolicyService.ExecuteAsync(async () =>
  {
      using (var client = new PaymentGatewayClient(...))
      {
          return await client.bpPayRequestAsync(...);
      }
  }, "BehPardakht-GetToken");
  ```

---

### Phase 2: Enhanced Logging Infrastructure

**Priority**: 🟠 **HIGH**

**LogService.cs Extension**:

**New Methods**:
```csharp
public void LogServiceCall(ServiceCallLogModel log)
{
    // Standardized structured logging
    using (LogContext.PushProperty("ServiceCall", log, true))
    {
        _logger.LogInformation("[ServiceCall] {@ServiceCall}", log);
    }
}

public void LogTimeout(string serviceName, string operationName, TimeSpan duration, string requestBody, string? partialResponse)
{
    var timeoutLog = new TimeoutLogModel
    {
        ServiceName = serviceName,
        OperationName = operationName,
        Duration = duration.TotalMilliseconds,
        RequestBody = TruncateBody(requestBody, 3000),
        PartialResponseBody = TruncateBody(partialResponse, 2000),
        Timestamp = DateTime.UtcNow,
        RequestId = GetCorrelationId()
    };

    using (LogContext.PushProperty("Timeout", timeoutLog, true))
    {
        _logger.LogWarning("[TIMEOUT] {ServiceName}.{OperationName} after {Duration}ms | RequestId: {RequestId}",
            serviceName, operationName, duration.TotalMilliseconds, timeoutLog.RequestId);
    }
}
```

**New Model**: `ServiceCallLogModel`
```csharp
public class ServiceCallLogModel
{
    public string ServiceName { get; set; }           // Provider name or handler name
    public ServiceTypeEnum ServiceType { get; set; }  // Internal | External
    public string OperationName { get; set; }         // GetToken, VerifyTransaction, etc.
    public Guid RequestId { get; set; }               // Correlation ID
    public long DurationMs { get; set; }              // Duration in milliseconds
    public int Status { get; set; }                   // HTTP status or custom status
    public string RequestBody { get; set; }           // Truncated at 3000 chars
    public string ResponseBody { get; set; }          // Truncated at 2000 chars
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
}
```

**SOAP Logging Wrapper**:
```csharp
public interface ISoapLoggingWrapper
{
    Task<TResponse> ExecuteWithLoggingAsync<TRequest, TResponse>(
        string serviceName,
        string operationName,
        TRequest request,
        Func<Task<TResponse>> soapCall);
}
```

---

### Phase 3: Infrastructure Service Logging

**Priority**: 🟡 **MEDIUM**

**DatabaseLoggingInterceptor**:
- Location: `src/CPG.Infrastructure.Persistence/Interceptors/DatabaseLoggingInterceptor.cs`
- Implements: `DbCommandInterceptor`
- Features:
  - Log all queries with duration, context type (Read/Write), row count
  - Slow query detection (threshold from config)
  - N+1 pattern detection (track sequential queries in same request context)
  - Parameterized query logging (mask sensitive values)

**MinIO Logging Enhancement**:
- Extend `MinioProvider` methods to log:
  - Operation type (PUT, GET, DELETE)
  - Bucket name, object key
  - File size (bytes)
  - Duration (ms)
  - Success/failure status
- Use structured logging via LogService

---

## Risks and Mitigations

### Risk 1: Performance Impact of Full Request/Response Logging

**Risk Level**: 🟠 **MEDIUM**

**Mitigation**:
- Async logging with buffering (Serilog async sinks)
- Truncation strategy (3000 chars request, 2000 chars response)
- Conditional logging: Full bodies only on timeout/error, summary on success (configurable)
- Performance testing under load (>1000 TPS) to validate <50ms latency impact

### Risk 2: Circuit Breaker State Persistence Across Restarts

**Risk Level**: 🟡 **LOW**

**Mitigation**:
- Polly v8.x supports state persistence via custom storage
- Implement Redis-based circuit breaker state storage
- Alternative: Accept transient state (reset on restart) as acceptable for first iteration

### Risk 3: Sensitive Data Exposure in Logs

**Risk Level**: 🔴 **CRITICAL**

**Mitigation**:
- Extend existing regex masking (Constants.Pattern) to cover:
  - PAN: Mask all but last 4 digits
  - CVV2: Complete redaction
  - Access tokens: Truncate to first 10 chars + "..."
- Automated testing for PCI-DSS compliance
- Security audit before production deployment

---

## Technical Decisions

### Decision 1: Polly v8.x Resilience Pipeline vs Legacy Policy API

**Chosen**: Resilience Pipeline (v8.x API)

**Rationale**:
- Modern API with better composition
- Built-in telemetry support
- Easier per-provider configuration
- Better async/await support

### Decision 2: Centralized vs Per-Provider Retry Configuration

**Chosen**: Centralized with per-provider overrides

**Rationale**:
- Default policies in `appsettings.json:Infrastructure:Resilience`
- Provider-specific overrides in `appsettings.json:Infrastructure:Resilience:Providers:{ProviderName}`
- Easier operational management
- Consistent behavior by default

### Decision 3: SOAP Logging - Envelope vs Serialized Objects

**Chosen**: Serialized objects with XML envelope on timeout

**Rationale**:
- Normal operations: JSON-serialized objects (easier to parse in Elasticsearch)
- Timeout scenarios: Include full SOAP XML envelope for deep debugging
- Balance between usability and completeness

### Decision 4: Database Logging - All Queries vs Slow Queries Only

**Chosen**: All queries with tiered log levels

**Rationale**:
- Information level: All queries with duration
- Warning level: Slow queries (>5s configurable threshold)
- Warning level: Suspected N+1 patterns
- Allows post-hoc analysis without pre-filtering

---

## Open Questions

### Q1: Task 18535 Log Format Specification

**Status**: ⚠️ **NOT FOUND IN CODEBASE**

**Action**: Request task 18535 documentation from team
- Referenced in Spec.md:35 and FR-010
- Need exact field names, data types, Elasticsearch mapping

### Q2: Correlation ID Generation Strategy

**Options**:
1. Generate at API middleware layer (earliest point)
2. Use ASP.NET Core built-in TraceIdentifier
3. Custom GUID generation with "CPG-" prefix

**Recommendation**: Use `HttpContext.TraceIdentifier` (built-in, standardized)

### Q3: Polly Policy Scope - Per Operation or Per Provider?

**Options**:
1. Single policy for all HTTP operations
2. Per-provider policies (AsanPardakht, BehPardakht, etc.)
3. Per-operation-type policies (Token, Verify, Settle)

**Recommendation**: Per-provider with operation-type overrides
- Most flexibility
- Aligns with current provider-specific retry counts

---

## Next Steps

### Immediate Actions (Tech.md Phase)
1. Define detailed class structure for `IPollyPolicyService` and `PollyPolicyService`
2. Design `ServiceCallLogModel` schema aligned with Elasticsearch mapping
3. Create database interceptor implementation plan
4. Define appsettings.json configuration schema

### Dependencies
- **External**: Task 18535 log format specification
- **Internal**: Confirm Redis availability for circuit breaker state (if persistence required)

### Acceptance Criteria for Research Completion
- ✅ Current state documented with code references
- ✅ All 4 feature tasks covered (Retry, Logging, Categorization, Infrastructure)
- ✅ Technology stack assessed (Polly, Serilog, EF Core, MinIO)
- ✅ Implementation recommendations provided
- ✅ Risks identified with mitigations
- ✅ Technical decisions documented with rationale
- ⏳ Open questions flagged (awaiting task 18535 spec)

---

**Research Status**: ✅ **COMPLETE - Ready for Technical Architecture Planning**
