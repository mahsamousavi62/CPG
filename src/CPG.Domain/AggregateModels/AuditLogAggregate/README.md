# AuditLog Aggregate - Structured Logging Domain Models

## Overview

The AuditLog aggregate provides a comprehensive 25-parameter structured logging system for the CPG payment gateway. It follows Clean Architecture principles and DDD patterns, placing domain models in the Domain layer with infrastructure concerns separated.

## Architecture Decision

**Location**: `CPG.Domain/AggregateModels/AuditLogAggregate/`

This aggregate lives in the Domain layer because:
- Represents core business concepts (audit, compliance, observability)
- Contains domain logic and value objects
- Infrastructure (Serilog, Elasticsearch) depends on these models, not vice versa
- Follows the same pattern as other aggregates (Payment, Company, etc.)

## Domain Models

### 1. AuditLog (Main Aggregate)

The primary domain model containing all 25 structured logging parameters.

**Categories**:
- **Core Identification (3)**: LogId, CorrelationId, Level
- **Service Context (4)**: ServiceName, ServiceType, ProviderName, ProviderType
- **User/Client Context (7)**: AuditType, UserId, Ip, UserAgent, ApplicationId, CompanyId, ClientId
- **HTTP Request/Response (6)**: RequestUri, RequestHeader, RequestBody, ResponseStatusCode, ResponseHeader, ResponseBody
- **Timing (3)**: StartDateTime, EndDateTime, DurationMs
- **Error Information (2)**: ErrorCode, ErrorType

**Usage**:
```csharp
var auditLog = new AuditLog(
    logId: new LogId("12345", "Payment", "IPG"),
    correlationId: "abc-123-def",
    level: AuditLevel.Information,
    serviceName: "AsanPardakhtToken",
    auditType: AuditType.Provider,
    startDateTime: startTime,
    endDateTime: endTime,
    durationMs: 1500,
    // ... additional optional parameters
);
```

### 2. LogId (Value Object)

Structured log identifier with format: `{Id}-{Domain}-{Abbreviation}`

**Examples**:
- `12345-Payment-IPG`
- `67890-DirectDebit-Vandar`
- `54321-CharismaCard-CC`

**Usage**:
```csharp
// Construction
var logId = new LogId("12345", "Payment", "IPG");
var logId = new LogId("12345-Payment-IPG");

// Parsing
var (id, domain, abbreviation) = logId.Parse();

// Implicit conversion
string logIdString = logId; // "12345-Payment-IPG"
LogId logId = "12345-Payment-IPG";
```

### 3. AuditLevel (Enum)

Severity level for audit logs:
- `Information` (1): Successful operations
- `Warning` (2): Non-critical issues
- `Error` (3): Failures and exceptions

### 4. HttpRequestLog (Record)

HTTP request information for audit logging.

**Properties**:
- `Uri`: Request URI
- `Header`: JSON serialized headers
- `Body`: Request body (truncatable)
- `Method`: HTTP method (GET, POST, etc.)
- `QueryString`: Query parameters

**Usage**:
```csharp
var request = new HttpRequestLog(
    uri: "https://api.example.com/token",
    header: "{\"Content-Type\":\"application/json\"}",
    body: "{\"amount\":1000}",
    method: "POST"
);

// Truncate large bodies for logging
var truncated = request.WithTruncatedBody(maxLength: 3000);
```

### 5. HttpResponseLog (Record)

HTTP response information for audit logging.

**Properties**:
- `StatusCode`: HTTP status code
- `Header`: JSON serialized headers
- `Body`: Response body (truncatable)
- `IsSuccess`: Computed property (2xx status)

**Usage**:
```csharp
var response = new HttpResponseLog(
    statusCode: 200,
    header: "{\"Content-Type\":\"application/json\"}",
    body: "{\"token\":\"xyz\",\"status\":\"success\"}"
);

if (response.IsSuccess)
{
    // Handle success
}
```

### 6. ServiceContext (Record)

Service and provider context information.

**Properties**:
- `ServiceName`: Name of service being called
- `ServiceType`: Type enum (IPG, DirectDebit, etc.)
- `ProviderName`: Provider name string
- `ProviderType`: Provider type enum

**Usage**:
```csharp
// Internal service
var context = ServiceContext.ForInternalService(
    "PaymentProcessing",
    ServiceType.AsanPardakhtToken
);

// External provider
var context = ServiceContext.ForProvider(
    "AsanPardakhtToken",
    ProviderTypeInLog.AsanPardakht,
    ServiceType.AsanPardakhtToken
);
```

### 7. UserContext (Record)

User and client context information.

**Properties**:
- `UserId`: User identifier
- `Ip`: Client IP address
- `UserAgent`: Browser/client user agent
- `ApplicationId`: Application identifier
- `CompanyId`: Company identifier
- `ClientId`: OAuth/IDP client ID
- `MobilePhone`: Mobile phone number

**Usage**:
```csharp
// Anonymous request
var context = UserContext.Anonymous(
    ip: "192.168.1.1",
    userAgent: "Mozilla/5.0..."
);

// Authenticated user
var context = UserContext.ForUser(
    userId: 12345,
    ip: "192.168.1.1",
    applicationId: 100,
    companyId: 50
);

// Application/client request
var context = UserContext.ForClient(
    applicationId: 100,
    clientId: "client-123",
    companyId: 50
);
```

### 8. TimingInfo (Record)

Timing information for operations.

**Properties**:
- `StartDateTime`: Operation start time
- `EndDateTime`: Operation end time
- `DurationMs`: Duration in milliseconds

**Usage**:
```csharp
// From start/end times
var timing = new TimingInfo(startTime, endTime);

// From start time (calculates to now)
var timing = TimingInfo.FromStart(startTime);

// With explicit duration
var timing = TimingInfo.WithDuration(startTime, 1500);

// Check threshold
if (timing.ExceededThreshold(30000))
{
    // Log timeout
}
```

### 9. ErrorInfo (Record)

Error information for failed operations.

**Properties**:
- `ErrorCode`: Error code (HTTP status, business code)
- `ErrorType`: Error category
- `StackTrace`: Exception stack trace
- `Message`: Error message

**Usage**:
```csharp
// From exception
var error = ErrorInfo.FromException(
    exception,
    errorCode: "PAYMENT_FAILED"
);

// From HTTP error
var error = ErrorInfo.FromHttpError(
    statusCode: 500,
    reasonPhrase: "Internal Server Error"
);

// Business error
var error = ErrorInfo.FromBusinessError(
    errorCode: "INVALID_AMOUNT",
    message: "Amount must be greater than zero"
);
```

### 10. AuditLogBuilder (Builder Pattern)

Fluent builder for constructing AuditLog instances.

**Usage**:
```csharp
var auditLog = AuditLogBuilder.Create()
    .WithLogId("12345", "Payment", "IPG")
    .WithCorrelationId("abc-123-def")
    .WithServiceName("AsanPardakhtToken")
    .WithAuditType(AuditType.Provider)
    .WithServiceContext(serviceContext)
    .WithUserContext(userContext)
    .WithRequest(httpRequest)
    .WithResponse(httpResponse)
    .WithTiming(timingInfo)
    .AsSuccess()
    .Build();

// Error scenario
var errorLog = AuditLogBuilder.Create()
    .WithLogId("12345", "Payment", "IPG")
    .WithCorrelationId("abc-123-def")
    .WithServiceName("AsanPardakhtToken")
    .WithAuditType(AuditType.Provider)
    .WithStartDateTime(startTime)
    .WithRequest(httpRequest)
    .AsError(exception, "TIMEOUT")
    .Build();
```

## Infrastructure Integration

### ILogger Extensions

Located in: `CPG.Infrastructure/Logging/Extensions/AuditLogExtensions.cs`

**Usage**:
```csharp
using CPG.Infrastructure.Logging.Extensions;

// Log audit entry
_logger.LogAudit(auditLog);

// Log success
_logger.LogAuditSuccess(auditLog);

// Log error
_logger.LogAuditError(auditLog, exception);

// Log warning
_logger.LogAuditWarning(auditLog);

// Log timeout with enhanced details
_logger.LogAuditTimeout(auditLog, timeoutException);

// Scoped logging with correlation ID
using (_logger.BeginAuditScope(correlationId, logId))
{
    // All logs within scope will include correlation ID
    _logger.LogAudit(auditLog);
}
```

## Example: Complete Provider Call Logging

```csharp
public class AsanPardakhtProvider
{
    private readonly ILogger<AsanPardakhtProvider> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        var logId = new LogId(request.PaymentRequestId.ToString(), "Payment", "IPG");

        try
        {
            // Create service context
            var serviceContext = ServiceContext.ForProvider(
                "AsanPardakhtToken",
                ProviderTypeInLog.AsanPardakht,
                ServiceType.AsanPardakhtToken
            );

            // Create user context
            var userContext = UserContext.ForUser(
                userId: GetCurrentUserId(),
                ip: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"],
                companyId: request.CompanyId
            );

            // Create HTTP request log
            var httpRequest = new HttpRequestLog(
                uri: "https://api.asanpardakht.ir/token",
                header: JsonSerializer.Serialize(GetHeaders()),
                body: JsonSerializer.Serialize(request),
                method: "POST"
            );

            // Call provider
            var response = await CallProviderAsync(request);

            // Create HTTP response log
            var httpResponse = new HttpResponseLog(
                statusCode: 200,
                body: JsonSerializer.Serialize(response)
            );

            // Create timing info
            var timing = TimingInfo.FromStart(startTime);

            // Build and log success
            var auditLog = AuditLogBuilder.Create()
                .WithLogId(logId)
                .WithCorrelationId(correlationId)
                .WithServiceContext(serviceContext)
                .WithUserContext(userContext)
                .WithRequest(httpRequest.WithTruncatedBody())
                .WithResponse(httpResponse.WithTruncatedBody())
                .WithTiming(timing)
                .WithAuditType(AuditType.Provider)
                .AsSuccess()
                .Build();

            _logger.LogAuditSuccess(auditLog);

            return response;
        }
        catch (TimeoutException ex)
        {
            // Log timeout with full request/response
            var timing = TimingInfo.FromStart(startTime);
            var error = ErrorInfo.FromException(ex, "TIMEOUT");

            var auditLog = AuditLogBuilder.Create()
                .WithLogId(logId)
                .WithCorrelationId(correlationId)
                .WithServiceName("AsanPardakhtToken")
                .WithAuditType(AuditType.Provider)
                .WithProvider(ProviderTypeInLog.AsanPardakht)
                .WithUserContext(userContext)
                .WithRequest(httpRequest)
                .WithTiming(timing)
                .WithError(error)
                .Build();

            _logger.LogAuditTimeout(auditLog, ex);
            throw;
        }
        catch (Exception ex)
        {
            // Log general error
            var timing = TimingInfo.FromStart(startTime);
            var error = ErrorInfo.FromException(ex);

            var auditLog = AuditLogBuilder.Create()
                .WithLogId(logId)
                .WithCorrelationId(correlationId)
                .WithServiceName("AsanPardakhtToken")
                .WithAuditType(AuditType.Provider)
                .WithProvider(ProviderTypeInLog.AsanPardakht)
                .WithUserContext(userContext)
                .WithRequest(httpRequest)
                .WithTiming(timing)
                .WithError(error)
                .Build();

            _logger.LogAuditError(auditLog, ex);
            throw;
        }
    }
}
```

## Elasticsearch Integration

All audit logs are automatically indexed in Elasticsearch via Serilog enrichers. The structured format ensures:

- Easy querying by any of the 25 parameters
- Correlation ID tracing across services
- Performance analysis via timing data
- Error pattern detection
- User activity tracking
- Provider performance monitoring

**Example Elasticsearch Queries**:

```json
// Find all timeout errors
{
  "query": {
    "bool": {
      "must": [
        { "term": { "Level": "Error" } },
        { "term": { "ErrorType": "Timeout" } }
      ]
    }
  }
}

// Find slow operations (>5 seconds)
{
  "query": {
    "range": {
      "DurationMs": { "gte": 5000 }
    }
  }
}

// Trace by correlation ID
{
  "query": {
    "term": { "CorrelationId": "abc-123-def" }
  }
}
```

## Best Practices

1. **Always use correlation IDs** to trace requests across services
2. **Truncate large bodies** before logging to avoid index bloat
3. **Use appropriate audit types** (Client, Provider, User, Develop)
4. **Log both success and failure** for complete audit trail
5. **Include timing information** for performance analysis
6. **Use builder pattern** for clean, readable log construction
7. **Leverage value objects** (LogId, ErrorInfo, etc.) for consistency
8. **Apply structured logging** for Elasticsearch indexing

## Migration from Legacy Models

The new AuditLog aggregate replaces:
- `CallLogModel` in `CPG.Domain.SharedKernel.Logging`
- `RequestResponseLogModel` in `CPG.Domain.SharedKernel.Logging`

**Migration Strategy**:
1. New code should use AuditLog aggregate exclusively
2. Existing LogService can be gradually migrated to use AuditLogBuilder
3. Both models can coexist during transition period
4. Infrastructure logging remains in `CPG.Infrastructure/Logging`

## Related Documentation

- [CLAUDE.md](../../../../../../CLAUDE.md) - Project architecture and patterns
- [Polly Resilience Policies](../../../../../../CLAUDE.md#resilience-and-retry-policies)
- [Event Storming Board](https://miro.com/app/board/uXjVOZIABVo=/)
