# Feature Specification: Resilience and Observability Enhancement

**Feature Branch**: `feature/002-resilience-observability-enhancement`
**Created**: 2025-10-18
**Status**: Draft
**Input**:
"""
# لیست تسک‌ها (به ترتیب اولویت اجرا)

### 1️⃣ پیاده‌سازی **Retry Policy** برای کلیه سرویس‌های درگاه‌ها

پیاده‌سازی مکانیزم تکرار (Retry) برای سناریوهای حساس و خطاپذیر مانند:

* دریافت توکن
* استعلام تراکنش
* تأیید تراکنش
* تسویه تراکنش

> هدف: افزایش پایداری ارتباط با سرویس‌ها در مواقع خطاهای موقتی.

---

### 2️⃣ ثبت کامل **Request/Response Body** برای تمامی سرویس‌ها

ویژه سرویس‌های **SOAP** و **REST**، به‌خصوص در شرایطی که **Timeout** رخ می‌دهد.

> هدف: امکان ردیابی بهتر خطاها و بهبود عیب‌یابی.

---

### 3️⃣ دسته‌بندی و استانداردسازی **لاگ‌های سرویس‌های داخلی و خارجی**

* اطمینان از ثبت نام سرویس به‌درستی
* دسته‌بندی دقیق بر اساس سرویس
* هماهنگ با فرمت ارائه‌شده در تسک [18535](18535.md)

> 🛠 کلاس مرتبط: `LogService.cs`
> مسیر فایل: `../src/CPG.Infrastructure/Logging/LogService.cs`
> ➕ این کلاس را گسترش و کامل کن، نیاز به ایجاد کلاس جدید نیست.

---

### 4️⃣ ثبت لاگ برای سرویس‌های زیرساختی

افزودن لاگ برای تعاملات با سرویس‌هایی مانند:

* **MinIO**
* **Database (ORM یا مستقیم)**

> هدف: پوشش کامل مسیرهای داده و شناسایی گلوگاه‌ها یا خطاهای احتمالی.
"""

## User Scenarios & Testing *(mandatory)*

### Primary User Story
**As a** payment gateway operations team member, **I want** comprehensive resilience policies and detailed observability across all gateway services **so that** I can quickly diagnose and recover from transient failures, minimize payment processing downtime, and ensure reliable transaction handling even during external service disruptions.

**System Context**:
- **Infrastructure Layer**: Polly-based retry policies with exponential backoff for all HTTP and SOAP service calls (token retrieval, transaction inquiry/confirmation/settlement)
- **Observability**: Complete request/response body logging for all external service calls, with enhanced logging during timeout scenarios
- **Operational Excellence**: Standardized, categorized logging for all internal/external services enabling rapid troubleshooting and performance analysis
- **Data Flow Coverage**: Full visibility into infrastructure service interactions (MinIO file storage, database operations via EF Core)

### Acceptance Scenarios

#### Scenario 1: Retry Policy - Transient Failure Recovery
1. **Given** an external payment gateway service (AsanPardakht, BehPardakht, CharismaPay) experiences a temporary network timeout, **When** the CPG system calls the token retrieval endpoint, **Then** the system MUST automatically retry the request up to 3 times with exponential backoff before marking the operation as failed
   - **Happy Path**: First retry succeeds, token is retrieved, payment flow continues normally, retry attempt logged with duration
   - **Error Path**: All 3 retries fail, circuit breaker opens, fallback behavior triggered, comprehensive error logged with all retry attempts
   - **Edge Cases**: Service returns 5xx error on first attempt then succeeds on second retry; timeout on second attempt but succeeds on third

2. **Given** a transaction settlement service call is in progress, **When** the SOAP service becomes temporarily unavailable (503 Service Unavailable), **Then** the Polly retry policy MUST attempt the call 3 times with jittered exponential backoff and log each retry attempt
   - **Happy Path**: Second retry succeeds, settlement completes, transaction status updated to SettlementSucceeded (status 9)
   - **Error Path**: All retries exhausted, settlement marked as failed (status 10), PredictedSettlementDateTime calculated based on failure threshold
   - **Edge Cases**: Circuit breaker half-open state allows one test request; service recovers during retry sequence

#### Scenario 2: Complete Request/Response Logging on Timeout
3. **Given** a REST API call to NeoBank service is initiated, **When** the request times out after 30 seconds (default timeout), **Then** the system MUST log the complete HTTP request body (truncated at 3000 chars), response body (if partial), duration, request ID, and exception details
   - **Happy Path**: Timeout occurs, PollyLoggingHandler captures full request/response context, logs include correlation ID for distributed tracing
   - **Error Path**: Logging fails due to serialization error, fallback minimal logging captures exception type and timestamp
   - **Edge Cases**: Response body is binary/non-text, body exceeds truncation limit, nested timeout within retry policy

4. **Given** a SOAP service call (PaymentGatewayClient for BehPardakht) is in progress, **When** timeout occurs during ConfirmServiceSoapClient operation, **Then** SoapLoggingWrapper MUST log complete SOAP request envelope, response envelope (if any), operation name, service name, and timeout duration
   - **Happy Path**: Full SOAP XML request/response logged with proper formatting, searchable by operation name and service name
   - **Error Path**: SOAP envelope serialization fails, raw XML logged as fallback
   - **Edge Cases**: SOAP fault response received before timeout, partial response body available

#### Scenario 3: Standardized Log Categorization
5. **Given** the LogService.cs class receives a service call logging request, **When** logging an external service interaction (IPG provider), **Then** the log MUST include structured fields: ServiceName, ServiceType (Internal/External), OperationName, RequestId, Duration, Status, aligned with task 18535 format
   - **Happy Path**: All structured fields populated, logs queryable in Elasticsearch by service category, operation grouped by provider type
   - **Error Path**: Missing ServiceName defaults to "Unknown", warning logged about incomplete metadata
   - **Edge Cases**: Service call spans multiple internal operations, nested service calls require parent-child correlation

6. **Given** an internal service (MediatR command handler) processes a payment request, **When** the operation completes, **Then** LogService MUST categorize it as Internal service, include handler name, command type, execution time, and correlation ID
   - **Happy Path**: Internal service logs clearly separated from external provider logs, performance metrics aggregated by handler type
   - **Error Path**: Internal exception logged with full stack trace and categorized correctly for internal troubleshooting
   - **Edge Cases**: Internal service calls external service, both logged with same correlation ID but different categories

#### Scenario 4: Infrastructure Service Logging
7. **Given** the application uploads a payment receipt file to MinIO, **When** the PutObjectAsync operation executes, **Then** the system MUST log bucket name, object key, file size, upload duration, and operation status (success/failure)
   - **Happy Path**: Upload succeeds, log includes S3-compatible operation metadata, duration < 5s for files under 10MB
   - **Error Path**: Upload fails due to network error, retry policy applies, all attempts logged with failure reason
   - **Edge Cases**: Large file upload (>100MB) times out, partial upload logged with bytes transferred

8. **Given** Entity Framework Core executes a database query via ReadDbContext, **When** the query completes, **Then** DatabaseLoggingInterceptor MUST log query text (parameterized), execution time, row count, and context type (Read/Write)
   - **Happy Path**: Query execution < 100ms, logged at Information level with row count and duration
   - **Error Path**: Query timeout (>30s), logged at Warning level with full query text and parameters for analysis
   - **Edge Cases**: Long-running query identified (>5s), logged with performance warning; N+1 query pattern detected and flagged

### Edge Cases
- **Concurrent Retry Scenarios**: Multiple payment requests trigger retries simultaneously, circuit breaker state must be thread-safe and prevent cascading failures across concurrent operations
- **Logging Performance Impact**: High-volume traffic (>1000 TPS) combined with full request/response logging must not degrade payment processing latency beyond 50ms threshold
- **Log Rotation and Storage**: Complete request/response bodies logged for 7 days minimum, automatic cleanup prevents disk space exhaustion, logs archived to Elasticsearch with retention policy
- **Distributed Tracing Correlation**: Request spanning multiple services (API → Application → Infrastructure → External Provider) maintains consistent correlation ID across all log entries
- **Configuration Hot Reload**: Polly retry policy configuration changes (MaxRetryAttempts, timeout thresholds) applied without application restart
- **Circuit Breaker State Persistence**: Circuit breaker open/closed state persisted across application restarts to prevent immediate retry storms after deployment
- **Sensitive Data Sanitization**: Payment card details (PAN, CVV2), passwords, tokens automatically masked in logged request/response bodies per PCI-DSS compliance
- **Multi-Provider Scenarios**: Different retry policies per provider (AsanPardakht requires 5 retries, BehPardakht requires 3), configuration per provider type
- **Async Operation Logging**: Long-running async operations (settlement batch processing) logged with start/end markers and intermediate progress checkpoints
- **Error Aggregation**: Similar errors from same provider within 5-minute window aggregated to prevent log flooding, summary count included

## Requirements *(mandatory)*

### Functional Requirements

#### Retry Policy Requirements (Task 1)
- **FR-001**: System MUST implement automatic retry mechanism for all external payment gateway service calls including token retrieval, transaction inquiry, transaction confirmation, and transaction settlement with configurable retry count (default: 3 attempts)
  - *Success Criteria*: Service call retried exactly N times (configurable) when transient error occurs (timeout, 5xx, network error); succeeds on any retry attempt OR exhausts all attempts

- **FR-002**: System MUST apply exponential backoff with jitter for retry delays to prevent thundering herd problem when multiple payment requests retry simultaneously
  - *Success Criteria*: Delay between retries increases exponentially (e.g., 2s, 4s, 8s); random jitter (±20%) added to prevent synchronized retry storms; measurable via retry timestamps in logs

- **FR-003**: System MUST implement circuit breaker pattern to prevent continuous retry attempts when external service is completely unavailable
  - *Success Criteria*: After threshold failures (default: 5 within 30s window), circuit opens and fast-fails subsequent requests for configured duration (default: 60s); half-open state allows test request; circuit closes on success

- **FR-004**: System MUST log each retry attempt including retry number, delay duration, exception type, and service endpoint
  - *Success Criteria*: Log entry created for each retry with structured data: RetryAttempt, DelayMs, ExceptionType, ServiceName, Endpoint, Timestamp; queryable in log aggregation system

#### Request/Response Logging Requirements (Task 2)
- **FR-005**: System MUST log complete HTTP request body for all REST API calls to external services (CharismaPay, NeoBank, AsanPardakht) with automatic truncation at 3000 characters to prevent log storage issues
  - *Success Criteria*: Request body present in log entry for every external HTTP call; truncated at 3000 chars if longer; includes HTTP method, URL, headers (sensitive headers masked)

- **FR-006**: System MUST log complete HTTP response body for all REST API calls with automatic truncation at 2000-3000 characters, especially when timeout or error occurs
  - *Success Criteria*: Response body logged for successful calls and timeout/error scenarios; partial response logged if available; includes status code, response headers, duration in milliseconds

- **FR-007**: System MUST log complete SOAP request envelope for all SOAP service calls (BehPardakht PaymentGateway, PEC SaleService/ConfirmService) including operation name and service name
  - *Success Criteria*: Full SOAP XML envelope logged with proper structure; operation name and service name tagged for searchability; request body truncated at 3000 chars if needed

- **FR-008**: System MUST log complete SOAP response envelope (or fault) for all SOAP service calls, especially during timeout scenarios
  - *Success Criteria*: SOAP response XML logged including fault details if present; timeout scenarios capture partial response if available; includes operation duration

- **FR-009**: System MUST include correlation ID (Request ID) in all service call logs to enable distributed tracing across API → Application → Infrastructure → External Provider layers
  - *Success Criteria*: Unique correlation ID generated at API layer; propagated to all downstream service calls; present in all related log entries; searchable across distributed system

#### Log Categorization and Standardization Requirements (Task 3)
- **FR-010**: System MUST extend LogService.cs to support standardized structured logging format aligned with task 18535 specification including: ServiceName, ServiceType (Internal/External), OperationName, RequestId, Duration, Status
  - *Success Criteria*: LogService.cs provides methods for logging with all required structured fields; logs queryable by each field in Elasticsearch; backwards compatible with existing logging calls

- **FR-011**: System MUST categorize all external service calls with ServiceType="External" and provider-specific ServiceName (e.g., "AsanPardakht", "BehPardakht", "CharismaPay", "NeoBank", "IdpProvider")
  - *Success Criteria*: All external provider calls tagged with ServiceType=External; ServiceName matches provider type; logs filterable by provider in monitoring dashboards

- **FR-012**: System MUST categorize all internal service calls (MediatR handlers, domain events, application services) with ServiceType="Internal" and component-specific ServiceName (e.g., "PaymentRequestHandler", "TransactionService", "VerificationService")
  - *Success Criteria*: All internal operations tagged with ServiceType=Internal; ServiceName indicates handler/service type; clear separation between internal/external logs in queries

- **FR-013**: System MUST include OperationName for all service calls indicating the specific operation being performed (e.g., "GetToken", "VerifyTransaction", "SettleTransaction", "CreatePaymentRequest")
  - *Success Criteria*: Every log entry has OperationName; operation names consistent across similar calls; enables aggregation by operation type for performance analysis

- **FR-014**: System MUST record Duration in milliseconds for all service operations to enable performance monitoring and SLA tracking
  - *Success Criteria*: Duration calculated from operation start to completion; included in all service call logs; performance dashboards can aggregate by operation/service

#### Infrastructure Service Logging Requirements (Task 4)
- **FR-015**: System MUST log all MinIO file storage operations including bucket name, object key, file size, operation type (upload/download/delete), duration, and operation status (success/failure)
  - *Success Criteria*: Every MinIO operation logged with structured metadata; failed operations include error details; logs enable troubleshooting of file storage issues

- **FR-016**: System MUST log Entity Framework Core database operations via DatabaseLoggingInterceptor including query text (parameterized), execution time, row count, and context type (ReadDbContext/WriteDbContext)
  - *Success Criteria*: All EF Core queries logged with execution time; long-running queries (>5s) flagged at Warning level; query text parameterized to protect sensitive data; read/write operations distinguishable

- **FR-017**: System MUST detect and log slow database queries exceeding configurable threshold (default: 5 seconds) at Warning level to identify performance bottlenecks
  - *Success Criteria*: Queries exceeding threshold logged with full query text and execution plan metadata; performance degradation visible in monitoring dashboards; configurable threshold in appsettings.json

- **FR-018**: System MUST detect and flag potential N+1 query patterns in Entity Framework operations where multiple sequential queries could be optimized with eager loading
  - *Success Criteria*: Sequential queries within same request context detected; warning logged when N+1 pattern suspected; includes suggestion for Include/ThenInclude optimization

### Non-Functional Requirements
- **NFR-001**: Logging overhead MUST NOT increase payment processing latency by more than 50ms at p99 percentile under high load (>1000 TPS)
  - *Success Criteria*: Performance testing shows <50ms latency increase with full logging enabled; async logging used where possible; buffering prevents blocking operations

- **NFR-002**: Sensitive data (payment card numbers, CVV2, passwords, tokens) MUST be automatically masked in all logged request/response bodies per PCI-DSS compliance requirements
  - *Success Criteria*: Automated tests verify card numbers masked as "****1234"; CVV2 completely redacted; access tokens truncated; compliance audit passes

- **NFR-003**: Retry policy configuration (retry count, backoff strategy, circuit breaker thresholds) MUST be configurable per provider and per operation type without code changes
  - *Success Criteria*: Configuration in appsettings.json; hot-reload support; different settings for different providers (e.g., AsanPardakht vs BehPardakht)

- **NFR-004**: Log retention policy MUST maintain complete request/response logs for minimum 7 days in hot storage (Elasticsearch) with automatic archival to cold storage and cleanup to prevent disk exhaustion
  - *Success Criteria*: Logs queryable in Elasticsearch for 7 days; automatic archival after 7 days; cleanup job prevents disk usage >80%; retention configurable

## Scope Boundaries *(mandatory)*

### IN SCOPE
- **Retry Policy Implementation**: Automatic retry with exponential backoff and jitter for all external payment gateway service calls (token, inquiry, confirmation, settlement) using existing Polly infrastructure
- **Circuit Breaker Integration**: Circuit breaker pattern implementation to prevent cascading failures, configurable per provider with state persistence
- **Complete HTTP Logging**: Full request/response body logging for all REST API calls (CharismaPay, NeoBank, AsanPardakht) with timeout-specific enhancement via existing PollyLoggingHandler
- **Complete SOAP Logging**: Full SOAP envelope logging for all SOAP services (BehPardakht, PEC) via SoapLoggingWrapper with operation/service name tagging
- **LogService.cs Extension**: Extending existing LogService.cs class (src/CPG.Infrastructure/Logging/LogService.cs) to support standardized structured logging per task 18535 format
- **Internal/External Service Categorization**: Clear ServiceType tagging (Internal/External) with consistent ServiceName and OperationName across all service calls
- **MinIO Operation Logging**: Comprehensive logging for all file storage operations with S3-compatible metadata
- **Database Query Logging**: Enhanced DatabaseLoggingInterceptor with slow query detection, N+1 pattern warnings, and read/write context differentiation
- **Correlation ID Propagation**: End-to-end distributed tracing support across API → Application → Infrastructure → External Provider layers
- **Sensitive Data Masking**: Automatic PCI-DSS compliant data sanitization in all logged request/response bodies
- **Configuration Management**: Retry policy and logging threshold configuration via appsettings.json with hot-reload support

### OUT OF SCOPE
- **Custom Metrics/Monitoring Dashboards**: Creating new Grafana/Kibana dashboards or custom metric exporters (use existing Elasticsearch/Serilog infrastructure)
- **Real-time Alerting System**: Implementing new alerting mechanisms (e.g., PagerDuty, Slack notifications) - focus on logging only
- **Performance Optimization**: Refactoring existing service implementations for performance beyond logging overhead mitigation
- **New Provider Integrations**: Adding new payment gateway providers or modifying existing provider business logic
- **Distributed Tracing Infrastructure**: Implementing OpenTelemetry, Jaeger, or other distributed tracing platforms (use existing correlation ID approach)
- **Log Analytics Features**: Building log analysis tools, anomaly detection, or ML-based pattern recognition
- **API Rate Limiting**: Implementing new rate limiting or throttling mechanisms (retry policy handles backpressure only)
- **Database Performance Tuning**: Query optimization, index creation, or database schema changes beyond logging detection
- **Historical Data Migration**: Backfilling historical logs or migrating existing log formats to new standardized format
- **Multi-Region Failover**: Implementing geographic failover or multi-region retry strategies (single-region retry only)
- **Custom Authentication/Authorization**: Modifying security mechanisms for logging purposes
- **UI/Dashboard Development**: Creating user interfaces for log viewing or configuration management

---