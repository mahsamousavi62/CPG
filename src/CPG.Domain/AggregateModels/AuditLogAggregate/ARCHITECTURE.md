# AuditLog Aggregate - Architecture Diagram

## Domain Model Structure

```
CPG.Domain/AggregateModels/AuditLogAggregate/
├── AuditLog.cs                  [Main Aggregate - 25 Parameters]
│   ├── LogId (Value Object)
│   ├── CorrelationId (string)
│   ├── Level (AuditLevel enum)
│   ├── ServiceName (string)
│   ├── ServiceType (enum?)
│   ├── ProviderName (string?)
│   ├── ProviderType (enum?)
│   ├── AuditType (enum)
│   ├── UserId (long?)
│   ├── Ip (string?)
│   ├── UserAgent (string?)
│   ├── ApplicationId (long?)
│   ├── CompanyId (long?)
│   ├── ClientId (string?)
│   ├── RequestUri (string?)
│   ├── RequestHeader (string?)
│   ├── RequestBody (string?)
│   ├── ResponseStatusCode (int?)
│   ├── ResponseHeader (string?)
│   ├── ResponseBody (string?)
│   ├── StartDateTime (DateTime)
│   ├── EndDateTime (DateTime)
│   ├── DurationMs (long)
│   ├── ErrorCode (string?)
│   └── ErrorType (string?)
│
├── LogId.cs                     [Value Object]
│   ├── Value (string)
│   ├── Format: {Id}-{Domain}-{Abbreviation}
│   └── Parse() method
│
├── AuditLevel.cs                [Enum]
│   ├── Information = 1
│   ├── Warning = 2
│   └── Error = 3
│
├── HttpRequestLog.cs            [Record - Request Data]
│   ├── Uri (string)
│   ├── Header (string?)
│   ├── Body (string?)
│   ├── Method (string?)
│   ├── QueryString (string?)
│   └── WithTruncatedBody() method
│
├── HttpResponseLog.cs           [Record - Response Data]
│   ├── StatusCode (int)
│   ├── Header (string?)
│   ├── Body (string?)
│   ├── IsSuccess (computed)
│   └── WithTruncatedBody() method
│
├── ServiceContext.cs            [Record - Service Info]
│   ├── ServiceName (string)
│   ├── ServiceType (enum?)
│   ├── ProviderName (string?)
│   ├── ProviderType (enum?)
│   ├── ForInternalService() factory
│   └── ForProvider() factory
│
├── UserContext.cs               [Record - User Info]
│   ├── UserId (long?)
│   ├── Ip (string?)
│   ├── UserAgent (string?)
│   ├── ApplicationId (long?)
│   ├── CompanyId (long?)
│   ├── ClientId (string?)
│   ├── MobilePhone (string?)
│   ├── Anonymous() factory
│   ├── ForUser() factory
│   └── ForClient() factory
│
├── TimingInfo.cs                [Record - Timing Data]
│   ├── StartDateTime (DateTime)
│   ├── EndDateTime (DateTime)
│   ├── DurationMs (long)
│   ├── FromStart() factory
│   ├── WithDuration() factory
│   └── ExceededThreshold() method
│
├── ErrorInfo.cs                 [Record - Error Data]
│   ├── ErrorCode (string?)
│   ├── ErrorType (string?)
│   ├── StackTrace (string?)
│   ├── Message (string?)
│   ├── FromException() factory
│   ├── FromHttpError() factory
│   ├── FromBusinessError() factory
│   └── WithTruncatedStackTrace() method
│
└── AuditLogBuilder.cs           [Builder Pattern]
    ├── WithLogId()
    ├── WithCorrelationId()
    ├── WithLevel()
    ├── WithServiceName()
    ├── WithAuditType()
    ├── WithServiceContext()
    ├── WithUserContext()
    ├── WithRequest()
    ├── WithResponse()
    ├── WithTiming()
    ├── WithError()
    ├── WithStartDateTime()
    ├── WithEndDateTime()
    ├── WithDuration()
    ├── WithUserId()
    ├── WithCompanyId()
    ├── WithApplicationId()
    ├── WithIp()
    ├── WithUserAgent()
    ├── WithServiceType()
    ├── WithProvider()
    ├── AsSuccess()
    ├── AsWarning()
    ├── AsError()
    └── Build()
```

## Infrastructure Integration

```
CPG.Infrastructure/Logging/Extensions/
└── AuditLogExtensions.cs        [ILogger Extensions]
    ├── LogAudit()
    ├── LogAuditSuccess()
    ├── LogAuditError()
    ├── LogAuditWarning()
    ├── LogAuditTimeout()
    └── BeginAuditScope()
```

## Layer Dependencies

```
┌──────────────────────────────────────────────┐
│           CPG.API (Presentation)             │
│  - Controllers, Middleware, Startup          │
└────────────────┬─────────────────────────────┘
                 │ depends on
                 ▼
┌──────────────────────────────────────────────┐
│      CPG.Infrastructure (External)           │
│  - Logging Extensions (AuditLogExtensions)   │
│  - Serilog Configuration                     │
│  - Elasticsearch Integration                 │
└────────────────┬─────────────────────────────┘
                 │ depends on
                 ▼
┌──────────────────────────────────────────────┐
│      CPG.Application (Use Cases)             │
│  - CQRS Handlers                             │
│  - Validation                                │
│  - Business Logic                            │
└────────────────┬─────────────────────────────┘
                 │ depends on
                 ▼
┌──────────────────────────────────────────────┐
│      CPG.Domain (Core - No Dependencies)     │
│  - AuditLog Aggregate                        │
│  - Value Objects                             │
│  - Domain Models                             │
│  - Business Rules                            │
└──────────────────────────────────────────────┘
```

## Data Flow - Provider Call Example

```
1. HTTP Request arrives
   ↓
2. Extract UserContext (IHttpContextAccessor)
   - UserId, IP, UserAgent, CompanyId
   ↓
3. Generate Correlation ID
   ↓
4. Create LogId
   - Format: {PaymentRequestId}-Payment-IPG
   ↓
5. Start Timer
   ↓
6. Build HttpRequestLog
   - Serialize request body
   - Capture headers
   ↓
7. Call Provider (via Polly)
   ↓
8. Build HttpResponseLog
   - Capture response
   - Record status code
   ↓
9. Calculate Timing
   - EndDateTime
   - DurationMs
   ↓
10. Build AuditLog
    - Use AuditLogBuilder
    - Combine all contexts
    ↓
11. Log via Extension Method
    - _logger.LogAuditSuccess(auditLog)
    ↓
12. Serilog Enrichment
    - Add correlation ID
    - Add log ID
    - Add structured properties
    ↓
13. Elasticsearch Indexing
    - All 25 parameters indexed
    - Searchable and queryable
```

## Usage Pattern - Builder API

```csharp
// Step 1: Create builder
var builder = AuditLogBuilder.Create();

// Step 2: Set required fields
builder
    .WithLogId("12345", "Payment", "IPG")
    .WithCorrelationId(correlationId)
    .WithServiceName("AsanPardakhtToken")
    .WithAuditType(AuditType.Provider);

// Step 3: Add contexts
builder
    .WithServiceContext(serviceContext)
    .WithUserContext(userContext);

// Step 4: Add request/response
builder
    .WithRequest(httpRequest)
    .WithResponse(httpResponse);

// Step 5: Add timing
builder.WithTiming(timingInfo);

// Step 6: Set success/error
builder.AsSuccess();
// OR
builder.AsError(exception);

// Step 7: Build
var auditLog = builder.Build();

// Step 8: Log
_logger.LogAudit(auditLog);
```

## Component Relationships

```
                  AuditLog (Aggregate Root)
                       │
         ┌─────────────┼─────────────┐
         │             │             │
         ▼             ▼             ▼
    ServiceContext  UserContext  TimingInfo
         │             │             │
         └─────────────┼─────────────┘
                       │
              AuditLogBuilder
                       │
                       ▼
              AuditLogExtensions
                       │
                       ▼
                   ILogger
                       │
                       ▼
                   Serilog
                       │
                       ▼
                 Elasticsearch
```

## Value Objects Composition

```
HttpRequestLog
├── Uri: string
├── Header: string?
├── Body: string?
├── Method: string?
└── QueryString: string?

HttpResponseLog
├── StatusCode: int
├── Header: string?
├── Body: string?
└── IsSuccess: bool (computed)

ServiceContext
├── ServiceName: string
├── ServiceType: enum?
├── ProviderName: string?
└── ProviderType: enum?

UserContext
├── UserId: long?
├── Ip: string?
├── UserAgent: string?
├── ApplicationId: long?
├── CompanyId: long?
├── ClientId: string?
└── MobilePhone: string?

TimingInfo
├── StartDateTime: DateTime
├── EndDateTime: DateTime
└── DurationMs: long (calculated)

ErrorInfo
├── ErrorCode: string?
├── ErrorType: string?
├── StackTrace: string?
└── Message: string?

LogId
└── Value: string (format: {Id}-{Domain}-{Abbreviation})
```

## Elasticsearch Index Mapping

```json
{
  "mappings": {
    "properties": {
      "LogId": { "type": "keyword" },
      "CorrelationId": { "type": "keyword" },
      "Level": { "type": "keyword" },
      "ServiceName": { "type": "keyword" },
      "ServiceType": { "type": "keyword" },
      "ProviderName": { "type": "keyword" },
      "ProviderType": { "type": "keyword" },
      "AuditType": { "type": "keyword" },
      "UserId": { "type": "long" },
      "Ip": { "type": "ip" },
      "UserAgent": { "type": "text" },
      "ApplicationId": { "type": "long" },
      "CompanyId": { "type": "long" },
      "ClientId": { "type": "keyword" },
      "RequestUri": { "type": "text" },
      "RequestHeader": { "type": "text" },
      "RequestBody": { "type": "text" },
      "ResponseStatusCode": { "type": "integer" },
      "ResponseHeader": { "type": "text" },
      "ResponseBody": { "type": "text" },
      "StartDateTime": { "type": "date" },
      "EndDateTime": { "type": "date" },
      "DurationMs": { "type": "long" },
      "ErrorCode": { "type": "keyword" },
      "ErrorType": { "type": "keyword" }
    }
  }
}
```

## Query Examples

### Find Timeouts
```json
{
  "query": {
    "bool": {
      "must": [
        { "term": { "ErrorType": "Timeout" } },
        { "range": { "DurationMs": { "gte": 30000 } } }
      ]
    }
  }
}
```

### Trace by Correlation ID
```json
{
  "query": {
    "term": { "CorrelationId": "abc-123-def" }
  }
}
```

### Find Slow Operations
```json
{
  "query": {
    "range": { "DurationMs": { "gte": 5000 } }
  },
  "sort": [
    { "DurationMs": { "order": "desc" } }
  ]
}
```

### Provider Performance
```json
{
  "query": {
    "bool": {
      "must": [
        { "term": { "ProviderType": "AsanPardakht" } },
        { "range": { "StartDateTime": { "gte": "now-1h" } } }
      ]
    }
  },
  "aggs": {
    "avg_duration": { "avg": { "field": "DurationMs" } },
    "max_duration": { "max": { "field": "DurationMs" } },
    "error_count": { "filter": { "term": { "Level": "Error" } } }
  }
}
```

## Design Principles Applied

1. **Single Responsibility**: Each model has one clear purpose
2. **Open/Closed**: Extensible via builder pattern, closed for modification
3. **Liskov Substitution**: Records are immutable and substitutable
4. **Interface Segregation**: Small, focused interfaces
5. **Dependency Inversion**: Domain doesn't depend on infrastructure

## SOLID Compliance

✅ **S**ingle Responsibility - Each class/record has one job
✅ **O**pen/Closed - Builder pattern allows extension
✅ **L**iskov Substitution - Records are immutable
✅ **I**nterface Segregation - Focused value objects
✅ **D**ependency Inversion - Domain is independent

## DDD Patterns Used

- ✅ Aggregate Root (AuditLog)
- ✅ Value Objects (LogId, HttpRequestLog, etc.)
- ✅ Factory Methods (static constructors)
- ✅ Builder Pattern (complex object construction)
- ✅ Immutability (records with init properties)
- ✅ Ubiquitous Language (domain terms)
