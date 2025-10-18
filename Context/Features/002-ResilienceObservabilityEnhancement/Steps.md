# Implementation Steps: Resilience and Observability Enhancement

**Created**: 2025-10-18
**Status**: Implementation Plan
**Prerequisites**: Completed business specification (Spec.md) and technical planning (Tech.md with Research.md)

## 🚨 CRITICAL: This File is Your Progress Tracker

**This Steps.md file serves as the authoritative source of truth for implementation progress across all development sessions.**

### Key Principles
- **Token limits are irrelevant** - Progress is tracked here, sessions are resumable
- **Never rush or take shortcuts** - Each step deserves proper attention and time
- **Session boundaries don't matter** - User can resume where this file shows progress
- **Steps.md is the real todo list** - Even if AI uses TodoWrite during a session, THIS file is what persists
- **Quality over speed** - Thoroughness is mandatory, optimization for token limits is forbidden
- **Check off progress here** - Mark tasks as complete in this file as they're finished

### How This Works
1. Each task has a checkbox: `- [ ] **S001** Task description`
2. As tasks complete, they're marked: `- [x] **S001** Task description`
3. AI ignores token limit concerns and works methodically through steps
4. If context usage gets high (>80%), AI suggests user runs `/compact` before continuing
5. If session ends: User starts new session and resumes (this file has all progress)
6. Take the time needed for each step - there's no rush to finish in one session

---

## Implementation Phases

### Phase 1: Infrastructure Setup (Polly Policies Foundation)

*Create core Polly infrastructure classes and logging handlers*

- [x] **S001** Create PollyExtensions.cs with retry policy Extension methods
  - **Path**: `src/CPG.Infrastructure/Policies/PollyExtensions.cs`
  - **Dependencies**: None
  - **Completed**: 2025-10-18
  - **Notes**:
    - Implement `AddStandardRetryPolicy(maxRetryAttempts)` extension for IHttpClientBuilder
    - Implement `ExecuteWithRetryAsync<T>` for manual SOAP/other usage
    - Use Polly v8.x resilience pipeline builder
    - Exponential backoff with jitter (2s base, Math.Pow(2, retryAttempt) + random jitter)
    - NO Circuit Breaker (per user requirement)
    - Handle: HttpRequestException, TimeoutException, CommunicationException (SOAP)
    - Handle HTTP: 5xx, 408 Request Timeout, 429 Too Many Requests
    - Log retry attempts with: RetryCount, Delay, ExceptionType, ServiceName

- [x] **S002** [P] Create PollyLoggingHandler.cs for HTTP request/response logging
  - **Path**: `src/CPG.Infrastructure/Policies/PollyLoggingHandler.cs`
  - **Dependencies**: None (parallel with S001)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Implement DelegatingHandler for HTTP pipeline
    - Inject: ILogger, ILogService, IHttpContextAccessor
    - Capture request/response bodies using Stopwatch
    - Use HttpContext.TraceIdentifier as correlation ID
    - Log full request/response ONLY on error or >25s duration
    - Truncate request body at 3000 chars, response at 2000 chars
    - Log timeout with: RequestId, Duration, Method, Uri, RequestBody
    - Call LogService.TruncateBody() and existing masking methods

- [x] **S003** [P] Create SoapLogger.cs for SOAP service logging
  - **Path**: `src/CPG.Infrastructure/Logging/SoapLogger.cs`
  - **Dependencies**: None (parallel with S001-S002)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Simple class (no interface) per project style
    - Inject: ILogger<SoapLogger>, ILogService
    - Method: `LogSoapCallAsync<TRequest, TResponse>(serviceName, operationName, request, Func<Task<TResponse>> soapCall)`
    - Use Stopwatch for duration tracking
    - Serialize SOAP request/response to JSON using Newtonsoft.Json
    - Call existing LogService.AddServiceCallLog() with truncated bodies
    - Determine ProviderTypeInLog from serviceName: "BehPardakht" → Pec, "PEC" → Pec, others → AsanPardakht
    - Catch TimeoutException and call LogService.LogTimeout()
    - Log successful calls at Information level with duration

**🏁 MILESTONE: Polly Infrastructure Created**
*Commit: "Add Polly retry infrastructure and logging handlers - PollyExtensions, PollyLoggingHandler, SoapLogger"*

### Phase 2: LogService Extensions (Enhance Existing Class)

*Extend existing LogService.cs with new methods for timeout logging and improved masking*

- [x] **S004** Extend LogService.cs with LogTimeout method
  - **Path**: `src/CPG.Infrastructure/Logging/LogService.cs` (modify existing file)
  - **Dependencies**: S001-S003 (needs TruncateBody, GetCorrelationId)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Add method: `LogTimeout(serviceName, operationName, duration, requestBody, partialResponse)`
    - Create anonymous object with: ServiceName, OperationName, DurationMs, RequestBody (truncated/masked), PartialResponseBody, Timestamp, RequestId
    - Use LogContext.PushProperty("Timeout", timeoutLog, true) per existing pattern
    - Log at Error level with structured message
    - Call existing MaskSensitiveData() before truncating

- [x] **S005** [P] Add TruncateBody method to LogService.cs
  - **Path**: `src/CPG.Infrastructure/Logging/LogService.cs` (modify existing file)
  - **Dependencies**: None (parallel with S004)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Public method: `TruncateBody(string body, int maxLength)`
    - Return original if null/empty or length <= maxLength
    - Otherwise: `body.Substring(0, maxLength) + $"... [بریده: {body.Length - maxLength} کاراکتر]"`
    - Used by PollyLoggingHandler, SoapLogger, and LogTimeout

- [x] **S006** [P] Add GetCorrelationId private method to LogService.cs
  - **Path**: `src/CPG.Infrastructure/Logging/LogService.cs` (modify existing file)
  - **Dependencies**: None (parallel with S004-S005)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Private method: `GetCorrelationId()`
    - Return: `_httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString()`
    - Inject IHttpContextAccessor if not already injected (check constructor)
    - Used by LogTimeout method

- [x] **S007** Enhance MaskSensitiveData method with PCI-DSS compliance
  - **Path**: `src/CPG.Infrastructure/Logging/LogService.cs` (modify existing method)
  - **Dependencies**: S004-S006 (same file modifications)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Keep existing MyRegex().Replace() call for current masking
    - Add PAN masking: Regex.Replace(@"\b(\d{12})(\d{4})\b", "************$2") - keep last 4 digits
    - Add CVV2 complete redaction: Regex.Replace(@"""cvv2?""\s*:\s*""\d{3,4}""", "\"cvv2\":\"***\"", IgnoreCase)
    - Add Token truncation to 10 chars: Regex.Replace(@"""(token|access_token)""\s*:\s*""([^""]{10})[^""]*""", "\"$1\":\"$2...\"", IgnoreCase)
    - Ensure method is still private as it currently is

**🏁 MILESTONE: LogService Enhanced**
*Commit: "Extend LogService with timeout logging, body truncation, correlation ID, and enhanced PCI-DSS masking"*

### Phase 3: Database Logging Interceptor

*Add EF Core interceptor for database query logging with slow query detection*

- [x] **S008** Create DatabaseLoggingInterceptor.cs
  - **Path**: `src/CPG.Infrastructure.Persistence/Interceptors/DatabaseLoggingInterceptor.cs`
  - **Dependencies**: S001-S007 (uses logging infrastructure)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Implement: `DbCommandInterceptor` from Microsoft.EntityFrameworkCore.Diagnostics
    - Inject: ILogger<DatabaseLoggingInterceptor>, IConfiguration
    - Read config: `_slowQueryThresholdMs = configuration.GetValue<int>("Infrastructure:Database:Logging:SlowQueryThresholdMs", 5000)`
    - Use Dictionary<DbCommand, QueryContext> to track execution start time and context type
    - Override: `ReaderExecutingAsync` - capture start time and eventData.Context.GetType().Name (ReadDbContext/WriteDbContext)
    - Override: `ReaderExecutedAsync` - log duration, rows affected, context type at Information level; warn if duration > threshold
    - Override: `NonQueryExecutedAsync` - log NonQuery operations with duration and rows affected
    - QueryContext class: StartTime (DateTime), ContextType (string)

- [x] **S009** Register DatabaseLoggingInterceptor in Persistence DI
  - **Path**: `src/CPG.Infrastructure.Persistence/DependencyInjection.cs` (or equivalent DI setup file)
  - **Dependencies**: S008
  - **Completed**: 2025-10-18
  - **Notes**:
    - Add: `services.AddSingleton<DatabaseLoggingInterceptor>();`
    - Modify WriteDbContext registration to include interceptor:
      ```csharp
      .AddInterceptors(
          serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
          serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>(),
          serviceProvider.GetRequiredService<DatabaseLoggingInterceptor>())
      ```
    - Do the same for ReadDbContext registration if separate
    - Ensure interceptor is added AFTER existing interceptors

- [x] **S010** [P] Add database logging configuration to appsettings.json
  - **Path**: `src/CPG.API/appsettings.json` (or appsettings.Development.json)
  - **Dependencies**: None (parallel with S008-S009)
  - **Completed**: 2025-10-18
  - **Notes**:
    - Add section:
      ```json
      "Infrastructure": {
        "Database": {
          "Logging": {
            "SlowQueryThresholdMs": 5000
          }
        }
      }
      ```
    - SlowQueryThresholdMs default: 5000 (5 seconds)
    - Consider different values for Development vs Production

**🏁 MILESTONE: Database Logging Complete**
*Commit: "Add DatabaseLoggingInterceptor for EF Core query logging and slow query detection"*

### Phase 4: MinIO Logging Enhancement

*Enhance existing MinioProvider with structured logging and duration tracking*

- [ ] **S011** Add Stopwatch and structured logging to MinioProvider.PutObject
  - **Path**: `src/CPG.Infrastructure/Minio/MinioProvider.cs` (modify existing method)
  - **Dependencies**: S004-S007 (LogService enhancements available)
  - **Notes**:
    - Add: `var stopwatch = Stopwatch.StartNew();` before PutObjectAsync call
    - Add: `stopwatch.Stop();` after successful PutObjectAsync AND in catch block
    - Replace existing `_logger.LogWarning` success log with:
      ```csharp
      _logger.LogInformation(
          "[MinIO] PutObject succeeded in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
          stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);
      ```
    - Enhance catch block to log failure with duration:
      ```csharp
      _logger.LogError(exc,
          "[MinIO] PutObject FAILED after {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
          stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);
      ```

- [ ] **S012** [P] Add Stopwatch and structured logging to MinioProvider.GetObject (if exists)
  - **Path**: `src/CPG.Infrastructure/Minio/MinioProvider.cs` (modify existing method)
  - **Dependencies**: None (parallel with S011)
  - **Notes**:
    - Follow same pattern as S011 for GetObject method
    - If GetObject doesn't exist, mark this task as complete with note "Method not present"
    - Log: "[MinIO] GetObject succeeded/FAILED in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey}"

- [ ] **S013** [P] Add Stopwatch and structured logging to MinioProvider.DeleteObject (if exists)
  - **Path**: `src/CPG.Infrastructure/Minio/MinioProvider.cs` (modify existing method)
  - **Dependencies**: None (parallel with S011-S012)
  - **Notes**:
    - Follow same pattern as S011 for DeleteObject method
    - If DeleteObject doesn't exist, mark as complete with note "Method not present"
    - Log: "[MinIO] DeleteObject succeeded/FAILED in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey}"

**🏁 MILESTONE: MinIO Logging Enhanced**
*Commit: "Add duration tracking and structured logging to MinIO file operations"*

### Phase 5: HttpClient Integration (Polly Retry Policies)

*Attach Polly retry policies and logging handlers to all existing HttpClients*

- [ ] **S014** Register PollyLoggingHandler and SoapLogger in DI
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S001-S003, S004-S007 (infrastructure classes created)
  - **Notes**:
    - Add: `services.AddScoped<PollyLoggingHandler>();`
    - Add: `services.AddScoped<SoapLogger>();`
    - Register BEFORE HttpClient registrations

- [ ] **S015** Add Polly retry policy to charisPayClient HttpClient
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs` (modify existing HttpClient registration)
  - **Dependencies**: S014
  - **Notes**:
    - Find: `services.AddHttpClient("charisPayClient", c => { ... })`
    - Add after closing brace: `.AddStandardRetryPolicy(maxRetryAttempts: 3)`
    - Add: `.AddHttpMessageHandler<PollyLoggingHandler>()`
    - Result:
      ```csharp
      services.AddHttpClient("charisPayClient", c => { ... })
          .AddStandardRetryPolicy(maxRetryAttempts: 3)
          .AddHttpMessageHandler<PollyLoggingHandler>();
      ```

- [ ] **S016** [P] Add Polly retry policy to idpClient HttpClient
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S014
  - **Notes**:
    - Find: `services.AddHttpClient("idpClient", c => { ... })`
    - Add: `.AddStandardRetryPolicy(maxRetryAttempts: 3).AddHttpMessageHandler<PollyLoggingHandler>()`
    - Same pattern as S015

- [ ] **S017** [P] Add Polly retry policy to neoBankClient HttpClient
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S014
  - **Notes**:
    - Find: `services.AddHttpClient("neoBankClient", c => { ... })`
    - Add: `.AddStandardRetryPolicy(maxRetryAttempts: 3).AddHttpMessageHandler<PollyLoggingHandler>()`

- [ ] **S018** [P] Add Polly retry policy to asanpardakhtClient HttpClient (5 retries)
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S014
  - **Notes**:
    - Find: `services.AddHttpClient("asanpardakhtClient", c => { ... })`
    - Add: `.AddStandardRetryPolicy(maxRetryAttempts: 5).AddHttpMessageHandler<PollyLoggingHandler>()`
    - NOTE: AsanPardakht requires 5 retries per existing code analysis

- [ ] **S019** [P] Add Polly retry policy to charismaCardClient HttpClient
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S014
  - **Notes**:
    - Find: `services.AddHttpClient("charismaCardClient", c => { ... })`
    - Add: `.AddStandardRetryPolicy(maxRetryAttempts: 3).AddHttpMessageHandler<PollyLoggingHandler>()`

- [ ] **S020** Verify default HttpClient also has retry policy (used by HttpProvider)
  - **Path**: `src/CPG.Infrastructure/DependencyInjection.cs`
  - **Dependencies**: S014-S019
  - **Notes**:
    - Check if there's a default HttpClient registration: `services.AddHttpClient()`
    - If exists, add: `.AddStandardRetryPolicy(maxRetryAttempts: 3).AddHttpMessageHandler<PollyLoggingHandler>()`
    - HttpProvider uses CreateClient() without name, so this handles it
    - If no default registration exists, add one:
      ```csharp
      services.AddHttpClient()
          .AddStandardRetryPolicy(maxRetryAttempts: 3)
          .AddHttpMessageHandler<PollyLoggingHandler>();
      ```

**🏁 MILESTONE: HttpClient Retry Policies Integrated**
*Commit: "Integrate Polly retry policies and logging handlers with all HttpClients"*

### Phase 6: SOAP Provider Integration (BehPardakht)

*Replace manual retry logic in BehPardakhtProvider with Polly + SoapLogger*

- [ ] **S021** Add SoapLogger dependency to BehPardakhtProvider constructor
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs`
  - **Dependencies**: S003, S014 (SoapLogger created and registered)
  - **Notes**:
    - Add private field: `private readonly SoapLogger _soapLogger;`
    - Add parameter to constructor: `SoapLogger soapLogger`
    - Assign in constructor: `_soapLogger = soapLogger;`
    - Constructor already has: ReadDbContext, IApplicationSettingsRepository, ILogService, ILogger

- [ ] **S022** Refactor GetPaymentTokenAsync to use Polly and SoapLogger
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs` (GetPaymentTokenAsync method)
  - **Dependencies**: S021
  - **Notes**:
    - REMOVE: try-catch block with tokenFailCounter manual retry logic
    - REMOVE: private field `tokenFailCounter` (no longer needed)
    - Wrap entire operation in: `PollyExtensions.ExecuteWithRetryAsync(async () => { ... }, _logger, 3, "BehPardakht")`
    - Inside retry, wrap SOAP call in: `_soapLogger.LogSoapCallAsync("BehPardakht", "GetPaymentToken", payRequest, async () => { ... })`
    - SOAP call lambda returns PaymentTokenResponse after processing response
    - Remove existing CreateLog() call (SoapLogger handles it)
    - Keep GetDataFromJsonProvider, CreateCallbackUrl, payRequest building logic unchanged

- [ ] **S023** [P] Refactor Verify method to use Polly and SoapLogger
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs` (Verify method)
  - **Dependencies**: S021
  - **Notes**:
    - REMOVE: try-catch block with verifyFailCounter manual retry
    - REMOVE: private field `verifyFailCounter`
    - Wrap in: `PollyExtensions.ExecuteWithRetryAsync` + `_soapLogger.LogSoapCallAsync("BehPardakht", "VerifyTransaction", verifyRequest, async () => { ... })`
    - Keep status parsing and VerifyTransactionResponse building logic unchanged
    - Remove existing CreateLog() call

- [ ] **S024** [P] Refactor Settle method to use Polly and SoapLogger
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs` (Settle method)
  - **Dependencies**: S021
  - **Notes**:
    - REMOVE: try-catch block with settleFailCounter manual retry
    - REMOVE: private field `settleFailCounter`
    - Wrap in: `PollyExtensions.ExecuteWithRetryAsync` + `_soapLogger.LogSoapCallAsync("BehPardakht", "SettleTransaction", settleRequest, async () => { ... })`
    - Keep status parsing and SettleTransactionResponse building logic unchanged
    - Remove existing CreateLog() call

- [ ] **S025** Remove unused manual retry infrastructure from BehPardakhtProvider
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs`
  - **Dependencies**: S022-S024 (all methods refactored)
  - **Notes**:
    - REMOVE: `private byte tokenFailCounter = 0;`
    - REMOVE: `private byte verifyFailCounter = 0;`
    - REMOVE: `private byte settleFailCounter = 0;`
    - REMOVE: `private readonly byte serviceCallMaxTryCounter = 5;`
    - Keep: CreateLog() helper if still used elsewhere (check first)
    - Keep: CreateAdditionalData(), CreateCallbackUrl(), GetDataFromJsonProvider() unchanged

**🏁 MILESTONE: BehPardakht Provider Modernized**
*Commit: "Replace manual retry logic with Polly and SoapLogger in BehPardakhtProvider"*

### Phase 7: SOAP Provider Integration (PEC Provider - if exists)

*Apply same refactoring to PecProvider if it exists in codebase*

- [ ] **S026** Verify PecProvider exists and identify SOAP methods needing refactoring
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/` (search for PecProvider.cs)
  - **Dependencies**: S021-S025 (BehPardakht pattern established)
  - **Notes**:
    - Use Glob or Grep to find PecProvider.cs
    - If NOT found: Mark this and S027-S029 as "N/A - PecProvider not present in codebase"
    - If found: Read file, identify SOAP service calls (likely SaleServiceSoapClient, ConfirmServiceSoapClient per Research.md)
    - Document methods needing refactoring in this task's completion notes

- [ ] **S027** Add SoapLogger to PecProvider and refactor sale/token methods
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/PecProvider.cs` (if exists)
  - **Dependencies**: S026
  - **Notes**:
    - Follow same pattern as S021-S022 for BehPardakht
    - Add SoapLogger dependency to constructor
    - Wrap sale/token methods in Polly + SoapLogger
    - Remove manual retry counters

- [ ] **S028** [P] Refactor PecProvider confirmation/settlement methods
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/PecProvider.cs` (if exists)
  - **Dependencies**: S026
  - **Notes**:
    - Apply same refactoring to remaining SOAP methods
    - Remove manual retry logic completely

- [ ] **S029** Clean up PecProvider manual retry infrastructure
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/PecProvider.cs` (if exists)
  - **Dependencies**: S027-S028
  - **Notes**:
    - Remove all manual retry counter fields
    - Remove serviceCallMaxTryCounter
    - Ensure CreateLog() calls removed (SoapLogger handles it)

**🏁 MILESTONE: All SOAP Providers Modernized**
*Commit: "Complete SOAP provider refactoring with Polly retry and SoapLogger integration"*

### Phase 8: AsanPardakht Provider Cleanup

*Remove manual retry logic from AsanPardakhtProvider (HTTP-based, already has Polly via HttpClient)*

- [ ] **S030** Verify AsanPardakhtProvider retry logic removal strategy
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`
  - **Dependencies**: S018 (asanpardakhtClient has Polly retry policy)
  - **Notes**:
    - AsanPardakhtProvider uses HttpProvider which uses IHttpClientFactory
    - HttpClient "asanpardakhtClient" already has Polly retry (S018) with 5 attempts
    - Manual retry in AsanPardakht: tokenFailCounter, verifyFailCounter, transactionResultFailCounter (max 5)
    - Strategy: Remove manual error handlers and retry logic, rely on HttpClient Polly policy
    - Review PaymentTokenErrorHandler, TransactionResultErrorHandler, VerifyErrorHandler methods

- [ ] **S031** Remove manual retry logic from AsanPardakhtProvider
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`
  - **Dependencies**: S030
  - **Notes**:
    - REMOVE: `private byte tokenFailCounter = 0;`
    - REMOVE: `private byte verifyFailCounter = 0;`
    - REMOVE: `private byte transactionResultFailCounter = 0;`
    - REMOVE: `private readonly byte serviceCallMaxTryCounter = 5;`
    - SIMPLIFY: PaymentTokenErrorHandler - remove retry logic, keep only error response handling
    - SIMPLIFY: TransactionResultErrorHandler - remove retry logic
    - SIMPLIFY: VerifyErrorHandler - remove retry logic
    - Keep: BaseErrorHandler (still needed for error response processing)
    - HttpClient retry policy now handles all retries automatically

**🏁 MILESTONE: AsanPardakht Provider Cleaned**
*Commit: "Remove manual retry logic from AsanPardakhtProvider, rely on HttpClient Polly policy"*

### Phase 9: Automated Build Validation

*Validate compilation and run automated build checks*

- [ ] **S032** Build solution and fix any compilation errors
  - **Path**: Entire solution
  - **Dependencies**: S001-S031 (all code changes complete)
  - **Notes**:
    - Run: `dotnet build CPG.sln --configuration Release`
    - Fix any compilation errors related to:
      - Missing using statements (Polly namespaces)
      - Constructor parameter mismatches
      - Method signature changes
    - Ensure no warnings introduced related to new code
    - Verify all projects compile successfully

- [ ] **S033** [P] Run architecture tests to validate layer dependencies
  - **Path**: `tests/CPG.Architecture.Tests/`
  - **Dependencies**: S032 (solution builds)
  - **Notes**:
    - Run: `dotnet test tests/CPG.Architecture.Tests/CPG.Architecture.Tests.csproj`
    - Verify no new architecture violations introduced
    - Check that new Policies/ folder follows existing Infrastructure patterns
    - Ensure DatabaseLoggingInterceptor in correct Persistence layer

- [ ] **S034** [P] Verify NuGet package dependencies are resolved
  - **Path**: Solution-wide
  - **Dependencies**: S032
  - **Notes**:
    - Check Polly v8.x packages referenced correctly
    - Verify Microsoft.Extensions.Http.Resilience package installed for AddStandardResilienceHandler
    - Run: `dotnet restore CPG.sln --configfile nuget.config`
    - Check for any package version conflicts

**🏁 MILESTONE: Build Validation Complete**
*Commit: "Validate build and resolve compilation issues"*

### Phase 10: Unit Testing (New Infrastructure Components)

*Create unit tests for new Polly and logging infrastructure*

- [ ] **S035** Create PollyExtensionsTests for retry policy behavior
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/Policies/PollyExtensionsTests.cs` (new file, create if test project exists)
  - **Dependencies**: S032-S034 (solution builds)
  - **Notes**:
    - If CPG.Infrastructure.Tests.Unit project doesn't exist, skip S035-S039 with note "Unit test project not present"
    - Test: `ExecuteWithRetryAsync` retries exactly N times on failure
    - Test: Exponential backoff increases delay between retries
    - Test: Jitter adds randomness to delay (verify delay not exactly 2^n)
    - Test: Success on second retry completes without error
    - Test: All retries exhausted throws original exception
    - Mock: ILogger to verify retry logging

- [ ] **S036** [P] Create PollyLoggingHandlerTests for HTTP logging behavior
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/Policies/PollyLoggingHandlerTests.cs`
  - **Dependencies**: S032-S034
  - **Notes**:
    - Test: Request/response bodies captured and truncated correctly
    - Test: Correlation ID from HttpContext.TraceIdentifier used
    - Test: Logging only occurs on error or >25s duration (success <25s not logged)
    - Test: Timeout exception logged with full request body
    - Mock: ILogService, IHttpContextAccessor, ILogger

- [ ] **S037** [P] Create SoapLoggerTests for SOAP logging
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/Logging/SoapLoggerTests.cs`
  - **Dependencies**: S032-S034
  - **Notes**:
    - Test: Successful SOAP call logged with duration
    - Test: Timeout calls LogService.LogTimeout()
    - Test: Request/response JSON serialization and truncation
    - Test: ProviderTypeInLog determined correctly from serviceName
    - Mock: ILogService, ILogger

- [ ] **S038** [P] Create LogServiceTests for new extension methods
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/Logging/LogServiceTests.cs` (or add to existing test file)
  - **Dependencies**: S032-S034
  - **Notes**:
    - Test: `TruncateBody` returns original if length <= maxLength
    - Test: `TruncateBody` truncates and adds Farsi suffix if too long
    - Test: `LogTimeout` creates structured log with all required fields
    - Test: Enhanced `MaskSensitiveData` masks PAN (keeps last 4 digits)
    - Test: Enhanced `MaskSensitiveData` completely redacts CVV2
    - Test: Enhanced `MaskSensitiveData` truncates tokens to 10 chars
    - Mock: ILogger, IHttpContextAccessor

- [ ] **S039** [P] Create DatabaseLoggingInterceptorTests
  - **Path**: `tests/CPG.Infrastructure.Persistence.Tests.Unit/Interceptors/DatabaseLoggingInterceptorTests.cs`
  - **Dependencies**: S032-S034
  - **Notes**:
    - Test: Query logged with duration and row count
    - Test: Slow query (>threshold) logged at Warning level
    - Test: Context type (ReadDbContext/WriteDbContext) captured correctly
    - Test: NonQuery operations logged with rows affected
    - Mock: ILogger, IConfiguration (provide SlowQueryThresholdMs)

**🏁 MILESTONE: Unit Tests Created**
*Commit: "Add unit tests for Polly extensions, logging handlers, and interceptors"*

### Phase 11: Integration Testing Preparation

*Manual integration testing instructions and validation scenarios*

- [ ] **S040** Integration Test 1: AsanPardakht Token Retrieval with Retry
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 INTEGRATION TEST - AsanPardakht Retry Policy
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Access to AsanPardakht test environment
  ║
  ║ **Test Steps**:
  ║ 1. Configure AsanPardakht test endpoint to simulate transient failure
  ║    (or use Fiddler/network proxy to inject 503 error on first 2 attempts)
  ║ 2. Execute payment request creation that triggers GetPaymentTokenAsync
  ║ 3. Monitor logs in console and Elasticsearch for retry attempts
  ║
  ║ **Expected Results**:
  ║ - First attempt fails with timeout/503 (logged at Warning level)
  ║ - Second retry logged with delay ~2-4s
  ║ - Third attempt succeeds and returns token
  ║ - HttpClient "asanpardakhtClient" Polly policy handles all retries
  ║ - PollyLoggingHandler logs full request/response on error
  ║ - Correlation ID consistent across all retry log entries
  ║
  ║ **Validation**:
  ║ - Check Elasticsearch for log entries with same RequestId/TraceIdentifier
  ║ - Verify retry count matches expected (up to 5 for AsanPardakht)
  ║ - Verify PAN/CVV2 masked in logged request bodies
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

- [ ] **S041** Integration Test 2: BehPardakht SOAP Settlement with SoapLogger
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 INTEGRATION TEST - BehPardakht SOAP Logging
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Access to BehPardakht test SOAP service
  ║
  ║ **Test Steps**:
  ║ 1. Execute a complete payment flow: GetToken → Verify → Settle
  ║ 2. Monitor logs for SOAP call entries from SoapLogger
  ║ 3. Check that manual retry counters (tokenFailCounter, etc.) are NOT used
  ║
  ║ **Expected Results**:
  ║ - Each SOAP operation logged with: ServiceName="BehPardakht", OperationName, Duration
  ║ - Request/response bodies truncated to 3000/2000 chars
  ║ - Polly retry policy handles failures (no manual retry logic)
  ║ - Timeout scenarios trigger LogService.LogTimeout() call
  ║ - SOAP XML serialized to JSON for logging
  ║
  ║ **Validation**:
  ║ - Grep logs for "[SOAP] BehPardakht.GetPaymentToken" and verify structured fields
  ║ - Verify no log entries from old CreateLog() method (removed)
  ║ - Verify sensitive data masked in SOAP request bodies
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

- [ ] **S042** Integration Test 3: Database Slow Query Detection
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 INTEGRATION TEST - Database Logging Interceptor
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Access to CPG database, ability to run slow query
  ║
  ║ **Test Steps**:
  ║ 1. Set SlowQueryThresholdMs to 1000 (1 second) in appsettings.json for testing
  ║ 2. Execute a query that takes >1s (e.g., large PaymentRequest query without indexes)
  ║ 3. Monitor logs for slow query warning
  ║
  ║ **Expected Results**:
  ║ - All EF Core queries logged with: ContextType (ReadDbContext/WriteDbContext), Duration, Rows
  ║ - Queries >1s (threshold) logged at Warning level with query text
  ║ - Normal queries (<1s) logged at Information level
  ║ - No sensitive data in parameterized query text
  ║
  ║ **Validation**:
  ║ - Search logs for "[Database] SLOW QUERY detected"
  ║ - Verify query text includes parameters but not actual values
  ║ - Verify ContextType correctly identifies Read vs Write operations
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

- [ ] **S043** Integration Test 4: MinIO Upload with Duration Tracking
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 INTEGRATION TEST - MinIO Logging Enhancement
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: MinIO service running, test file for upload
  ║
  ║ **Test Steps**:
  ║ 1. Execute file upload operation (e.g., payment receipt upload)
  ║ 2. Monitor logs for MinIO operation entry
  ║ 3. Simulate upload failure by stopping MinIO service mid-upload
  ║
  ║ **Expected Results**:
  ║ - Successful upload logged with: Bucket, ObjectKey, Size, DurationMs
  ║ - Failed upload logged with exception and duration
  ║ - Log format: "[MinIO] PutObject succeeded in {DurationMs}ms | Bucket: ..."
  ║ - No change to existing MinIO functionality, only logging enhanced
  ║
  ║ **Validation**:
  ║ - Verify upload duration logged in milliseconds
  ║ - Verify bucket and object key present in log for troubleshooting
  ║ - Verify file size logged for performance analysis
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

**🏁 MILESTONE: Integration Testing Complete**
*All integration tests passed, retry and logging working end-to-end*

### Phase 12: Performance and Compliance Validation

*Validate performance impact and PCI-DSS compliance*

- [ ] **S044** Performance Test: Measure logging overhead on payment processing latency
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 PERFORMANCE TEST - Logging Overhead
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Load testing tool (Apache JMeter, k6, or similar)
  ║
  ║ **Test Steps**:
  ║ 1. Configure baseline test: 1000 payment requests over 60s
  ║ 2. Measure p50, p95, p99 latency WITHOUT new logging (on old branch)
  ║ 3. Measure same metrics WITH new logging (on feature branch)
  ║ 4. Compare latency increase
  ║
  ║ **Success Criteria** (per NFR-001):
  ║ - p99 latency increase < 50ms
  ║ - p95 latency increase < 30ms
  ║ - No significant throughput degradation (<5%)
  ║
  ║ **Expected Results**:
  ║ - PollyLoggingHandler only logs on error/timeout (not all requests)
  ║ - Async logging via Serilog prevents blocking
  ║ - Database interceptor overhead minimal (<10ms per query)
  ║
  ║ **If Performance Issue Found**:
  ║ - Reduce request/response body truncation limits (3000 → 1000 chars)
  ║ - Disable success logging in PollyLoggingHandler (only log errors)
  ║ - Consider async file I/O for Serilog
  ║
  ║ Reply "✅ Passed (latency increase: Xms)" or "❌ Issues: [description]"
  ```

- [ ] **S045** PCI-DSS Compliance Test: Verify sensitive data masking in logs
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 COMPLIANCE TEST - Sensitive Data Masking
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Test payment with real-looking card data
  ║
  ║ **Test Steps**:
  ║ 1. Execute payment with test PAN: 5022291234567890 (16 digits)
  ║ 2. Include CVV2: 123 in request
  ║ 3. Receive token: "abc123def456ghi789" from provider
  ║ 4. Search ALL logs (Elasticsearch, console, file) for sensitive data
  ║
  ║ **Success Criteria** (per NFR-002):
  ║ - PAN masked as: "************7890" (last 4 digits only)
  ║ - CVV2 completely redacted: "cvv2":"***"
  ║ - Token truncated: "abc123def4..." (10 chars + ellipsis)
  ║ - Password fields never logged
  ║ - National ID encrypted in AdditionalData (existing AES encryption)
  ║
  ║ **Validation Queries**:
  ║ - Grep logs for "5022291234567890" → Should NOT be found
  ║ - Grep logs for "123" in CVV2 context → Should NOT be found
  ║ - Grep logs for full token string → Should only find truncated version
  ║
  ║ **Critical**: If any sensitive data found unmasked, STOP and fix MaskSensitiveData()
  ║
  ║ Reply "✅ Passed (no sensitive data leaked)" or "❌ SECURITY ISSUE: [description]"
  ```

- [ ] **S046** Elasticsearch Query Validation: Verify structured logging and searchability
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 OBSERVABILITY TEST - Log Searchability
  ═══════════════════════════════════════════════════
  ║
  ║ **Prerequisites**: Elasticsearch access, Kibana for queries
  ║
  ║ **Test Queries**:
  ║ 1. Find all AsanPardakht timeouts: `ServiceName:"AsanPardakht" AND Timeout:*`
  ║ 2. Find slow database queries: `message:"SLOW QUERY" AND DurationMs:>5000`
  ║ 3. Find all retries for specific request: `RequestId:"xyz" AND message:"Retry"`
  ║ 4. Find MinIO failures: `message:"MinIO" AND level:"Error"`
  ║
  ║ **Success Criteria**:
  ║ - All logs have structured fields: ServiceName, OperationName, DurationMs, RequestId
  ║ - Logs categorized by ServiceType (Internal/External)
  ║ - Correlation ID enables tracing across layers
  ║ - Performance metrics aggregatable by service/operation
  ║
  ║ **Expected Visualizations**:
  ║ - Average duration by ServiceName (identify slow providers)
  ║ - Retry rate by provider (identify unstable services)
  ║ - Error rate over time by OperationName
  ║
  ║ Reply "✅ Passed (all queries work)" or "❌ Issues: [description]"
  ```

**🏁 MILESTONE: Performance and Compliance Validated**
*Commit: "Complete performance and compliance validation - ready for production"*

### Phase 13: Documentation and Release Preparation

*Update documentation and prepare for merge to stage branch*

- [ ] **S047** Update CLAUDE.md with new Polly infrastructure documentation
  - **Path**: `CLAUDE.md`
  - **Dependencies**: All implementation complete
  - **Notes**:
    - Add section: "## Resilience and Retry Policies" after existing Technology Stack section
    - Document: PollyExtensions usage, PollyLoggingHandler, SoapLogger
    - Update: Common Commands section with retry policy testing commands
    - Document: Configuration in appsettings.json (SlowQueryThresholdMs)
    - Explain: How to add Polly retry to new HttpClients in future
    - Link: To Tech.md for detailed architecture

- [ ] **S048** [P] Update README.md if deployment instructions changed
  - **Path**: `README.md` (if exists at project root)
  - **Dependencies**: None (parallel with S047)
  - **Notes**:
    - Check if README.md exists, if not skip this task
    - Update: Dependencies section to mention Polly v8.x
    - Update: Configuration section for new appsettings.json settings
    - Add: Troubleshooting section for retry policy debugging

- [ ] **S049** Create migration notes for operations team
  - **Path**: `Context/Features/002-ResilienceObservabilityEnhancement/MIGRATION.md` (new file)
  - **Dependencies**: All testing complete
  - **Notes**:
    - Document: What changed (manual retry removed, Polly added)
    - List: New configuration options in appsettings.json
    - Explain: How to monitor retry behavior in Elasticsearch
    - Provide: Example log queries for troubleshooting
    - Warn: Potential latency increase (<50ms), within acceptable range
    - Include: Rollback procedure if issues arise in production

- [ ] **S050** Verify all milestones committed and branch ready for PR
  - **Path**: Git repository
  - **Dependencies**: S001-S049 complete
  - **Notes**:
    - Run: `git log --oneline` and verify all milestone commits present:
      - "Add Polly retry infrastructure and logging handlers"
      - "Extend LogService with timeout logging and PCI-DSS masking"
      - "Add DatabaseLoggingInterceptor for EF Core query logging"
      - "Add duration tracking to MinIO file operations"
      - "Integrate Polly retry policies with all HttpClients"
      - "Replace manual retry logic with Polly in BehPardakhtProvider"
      - "Complete SOAP provider refactoring"
      - "Remove manual retry logic from AsanPardakhtProvider"
      - "Validate build and resolve compilation issues"
      - "Add unit tests for Polly extensions and logging"
      - "Complete performance and compliance validation"
    - Ensure: No uncommitted changes
    - Verify: Feature branch up to date with latest stage branch

**🏁 MILESTONE: Documentation Complete and Branch Ready**
*Commit: "Add migration documentation and update project docs for resilience enhancement"*

---

## Implementation Time Estimation (AI-Assisted Development)

> **⚠️ ESTIMATION BASIS**: These estimates assume development with Claude Code (AI) executing implementation tasks with human review and guidance. Times reflect AI execution + human review cycles, not manual coding.

### Phase-by-Phase Review Time

**Phase 1-2: Infrastructure Setup** (S001-S007): ~2-3 hours
- AI creates Polly classes and LogService extensions quickly
- Human reviews: Retry logic correctness, logging patterns, PCI-DSS masking
- Testing: Manual verification of retry behavior

**Phase 3-4: Database and MinIO Logging** (S008-S013): ~1-2 hours
- AI implements interceptor and enhances MinIO logging
- Human reviews: EF Core integration, slow query threshold tuning
- Testing: Database query execution and MinIO operations

**Phase 5: HttpClient Integration** (S014-S020): ~1 hour
- AI adds Polly policies to all HttpClients systematically
- Human reviews: Configuration consistency across clients
- Quick validation: Compilation and DI registration

**Phase 6-8: Provider Refactoring** (S021-S031): ~3-4 hours
- AI removes manual retry logic and integrates Polly/SoapLogger
- Human reviews: Business logic preservation, SOAP call correctness
- Critical review: Ensure no behavior changes beyond retry mechanism

**Phase 9-10: Build Validation and Unit Tests** (S032-S039): ~2-3 hours
- AI fixes compilation errors and creates unit tests
- Human reviews: Test coverage, edge cases, mocking correctness
- Validation: All tests pass, architecture tests clean

**Phase 11-12: Integration and Performance Testing** (S040-S046): ~4-6 hours
- **Human-intensive**: Manual integration testing with real/test services
- Performance benchmarking requires human setup and analysis
- PCI-DSS compliance validation critical and careful

**Phase 13: Documentation** (S047-S050): ~1-2 hours
- AI drafts documentation updates
- Human reviews and refines for operations team clarity

### Total Estimated Review Time

**Core Development**: 10-14 hours (AI implementation + human review)
**Manual Testing**: 4-6 hours (integration, performance, compliance)
**Total**: 14-20 hours (~2-3 working days with breaks)

> **💡 TIME COMPOSITION**:
> - AI Implementation: ~20% (Claude Code writes code fast)
> - Human Code Review: ~30% (ensuring correctness and patterns)
> - Manual Testing: ~30% (integration tests, performance validation)
> - Correction Cycles: ~20% (fixing issues found during testing)

### Risk-Adjusted Time

**🟢 Low Risk Components** (Well-documented .NET/Polly APIs): Minimal correction cycles
- PollyExtensions, HttpClient integration: Standard patterns

**🟡 Medium Risk Components** (Integration-heavy): Some refinement likely
- SOAP provider refactoring: Ensure no business logic broken
- Database interceptor: EF Core version compatibility

**🔴 High Risk Components** (Requires careful validation): Multiple test cycles
- PCI-DSS compliance: Sensitive data masking MUST be perfect
- Performance impact: May need tuning if >50ms overhead

**Risk-Adjusted Total**: 16-24 hours (2-3 days if compliance issues found)

---

## Implementation Structure (Task Management)

### Task Numbering Convention
- **Format**: `S###` with sequential numbering (S001 to S050)
- **Parallel Markers**: `[P]` for tasks executable concurrently
- **Dependencies**: Explicit prerequisites listed for each task
- **File Paths**: Specific target files or directories

### Progress Tracking and Session Continuity
- **This file is the progress tracker** - Check off tasks as `[x]` when complete
- **Sessions are resumable** - New sessions read this file to continue where left off
- **Token limits don't matter** - Work spans multiple sessions seamlessly
- **Never rush** - Each step gets proper time for quality
- **TodoWrite is temporary** - Only Steps.md persists across sessions
- **Quality paramount** - No shortcuts for speed

### Parallel Execution Rules
- **Different files** = `[P]` parallel safe (e.g., S002, S003 can run together)
- **Same file modifications** = Sequential only (e.g., S004-S007 on LogService.cs)
- **Independent components** = `[P]` parallel safe (e.g., S016-S019 HttpClients)
- **Shared resources** = Sequential only (e.g., S021-S025 BehPardakhtProvider)
- **Tests with implementation** = Can run `[P]` (e.g., S035-S039 test different classes)

---

## Dependency Analysis

### Critical Path (Longest Dependency Chain)
S001 → S014 → S015 → S032 → S040 → S044 → S050
- Polly infrastructure → DI registration → HttpClient integration → Build → Integration test → Performance validation → Branch ready
- **Estimated Critical Path Time**: ~12-16 hours

### Parallel Opportunities
- **Infrastructure Creation**: S001, S002, S003 (all independent)
- **LogService Extensions**: S005, S006 (independent of S004)
- **HttpClient Integration**: S016, S017, S018, S019 (all independent after S014-S015)
- **Unit Tests**: S035, S036, S037, S038, S039 (all independent after S032)
- **Provider Refactoring**: S023, S024 (parallel within BehPardakht after S022)

### Technology Dependencies
- **Polly v8.x**: Required for all retry policy implementation (S001-S031)
- **Microsoft.Extensions.Http.Resilience**: Required for AddStandardResilienceHandler (S001, S015-S020)
- **Serilog**: Existing dependency for structured logging (S004-S007, S002)
- **Entity Framework Core 8.0**: Required for DatabaseLoggingInterceptor (S008-S009)
- **Newtonsoft.Json**: Existing dependency for SOAP serialization (S003, S022-S029)

---

## Completion Verification

### Implementation Completeness
- [ ] All user scenarios from Spec.md have corresponding implementation tasks?
  - Retry policy for transient failures (S001, S015-S031)
  - Complete request/response logging on timeout (S002, S003, S004)
  - Standardized log categorization (S004-S007)
  - Infrastructure service logging (S008-S013)
- [ ] All architectural components from Tech.md created?
  - PollyExtensions.cs (S001)
  - PollyLoggingHandler.cs (S002)
  - SoapLogger.cs (S003)
  - LogService extensions (S004-S007)
  - DatabaseLoggingInterceptor (S008-S009)
  - MinIO enhancements (S011-S013)
- [ ] Error handling and edge cases covered?
  - Timeout logging (S002, S003, S004)
  - Slow query detection (S008)
  - Retry exhaustion scenarios (S040-S041)
- [ ] Performance requirements addressed (NFR-001)?
  - Performance testing task (S044)
  - Logging overhead validation
- [ ] PCI-DSS compliance addressed (NFR-002)?
  - Sensitive data masking (S007)
  - Compliance testing task (S045)

### Quality Standards
- [ ] Each task specifies exact file paths and dependencies?
- [ ] Parallel markers `[P]` applied correctly for independent tasks?
- [ ] Test tasks included for all major components (S035-S039)?
- [ ] Manual integration tests cover critical scenarios (S040-S043)?
- [ ] Code follows existing CPG project patterns?
  - Simple Extension methods (not complex interfaces)
  - Extend existing classes (LogService) vs creating new ones
  - Match Farsi documentation style
  - Use existing Serilog LogContext.PushProperty pattern

### Release Readiness
- [ ] Build validation completed (S032-S034)?
- [ ] Unit tests created and passing (S035-S039)?
- [ ] Integration tests passed (S040-S043)?
- [ ] Performance validated <50ms overhead (S044)?
- [ ] PCI-DSS compliance validated (S045)?
- [ ] Documentation updated (S047-S049)?
- [ ] All milestones committed to git (S050)?

---

**Next Phase**: Run `/ctxk:impl:start-working` (in new chat session) to begin systematic development execution following this implementation plan.

---
