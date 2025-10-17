# Technical Research: Asan Pardakht Settlement Service Integration

**Created**: 2025-10-17
**Status**: Research Complete
**Prerequisites**: Completed business specification (Spec.md)

## Research Overview

### Research Scope
This research covers the integration of Asan Pardakht Settlement API into the existing CPG payment gateway system. Key areas investigated:
- Existing AsanPardakhtProvider implementation and extension points
- VerifyTransactionQueryHandler modification requirements
- IPG_Transaction and Transaction domain entities structure
- Asan Pardakht Settlement REST API specifications
- Existing Polly resilience policies and HttpClient configuration
- Status code mappings and business rules
- DateTime handling patterns in the codebase

### Key Findings Summary
- **AsanPardakhtProvider already exists** at `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs` with Verify() method implemented and Settle() stub (NotImplemented)
- **VerifyTransactionQueryHandler** at `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs` already handles BehPardakht settlement after verification (lines 103-118) - this pattern should be replicated for Asan Pardakht
- **HttpClient `asanpardakhtClient`** already configured with Polly policies in DependencyInjection.cs
- **IPGTransactionStatus enum** already defines status codes 9 (SettlementSucceeded) and 10 (SettlementFailed)
- **Provider authentication** uses `usr` and `pwd` headers extracted from CompanyIPG.ProviderData JSON
- **DateTime.Now** is used consistently throughout IPG transaction handlers for timestamp calculations

## Codebase Integration Analysis

### Existing Architecture Patterns

**Provider Pattern**:
- All IPG providers implement `IIpgProvider` interface
- Located in `src/CPG.Infrastructure/Providers/Ipg/`
- Methods: GetPaymentTokenAsync(), GetTransactionResult(), Verify(), Settle()
- AsanPardakhtProvider currently has Settle() as NotImplementedException stub

**Error Handling Pattern**:
- Each provider method has dedicated error handler (e.g., VerifyErrorHandler, PaymentTokenErrorHandler)
- Error handlers map HTTP status codes to domain-specific status enums
- Uses status code arrays (e.g., `VerificationSucceededCodes`, `VerificationFailedCodes`)

**Provider Data Extraction**:
- `GetDataFromJsonProvider(string providerData)` method parses JSON
- Extracts: merchantConfigurationId, userName, password from CompanyIPG.ProviderData
- Throws ParseCompanyIpgProviderDataException on failure

**HTTP Communication**:
- Uses `IHttpProvider` abstraction with generic PostAsync/GetAsync methods
- Named HttpClient: "asanpardakhtClient" with Polly policies
- Base URL: "https://ipgrest.asanpardakht.ir/"
- Headers set via `GetHeaders()` method returning List<(string Key, string? Value)>

### Related Existing Components

**Domain Models**:
- **IPGTransaction** (`src/CPG.Domain/AggregateModels/TransactionAggregate/IPGTransaction.cs`):
  - Properties: TrackId, Status (IPGTransactionStatus enum), CompanyIPGId, IPGToken, ProviderTrackerId, ReferenceNumber, VerificationDateTime
  - Inherits from AuditableEntity<long> (has CreationDate, ModificationDate from interceptor)

- **Transaction** (`src/CPG.Domain/AggregateModels/TransactionAggregate/Transaction.cs`):
  - Properties: PaymentRquestId, IPGTransactionId, Amount, Status (TransactionStatus enum), PredictedSettlementDateTime
  - Navigation: IPGTransaction, PaymentRequest, DestinationDeposit

- **CompanyIPG** (`src/CPG.Domain/AggregateModels/CompanyIPGAggregate/CompanyIPG.cs`):
  - Properties: CompanyId, ProviderId, IPGTypeId, ProviderData (JSON string)
  - Navigation: Company, Provider, IPGType, List<IPGTransaction>

**Enums** (`src/CPG.Domain/SharedKernel/Enums.cs`):
```csharp
public enum IPGTransactionStatus : byte
{
    WaitingForPspResponse = 0,
    FetchingResult = 1,
    SucceededAndWaitingForVerification = 2,
    Failed = 3,
    Expired = 4,
    Verifying = 5,
    VerificationSucceeded = 6,
    VerificationFailed = 7,
    WaitingForSettlementRequest = 8,
    SettlementSucceeded = 9,      // Target status for successful settlement
    SettlementFailed = 10,         // Target status for failed settlement
    Cancelling = 11,
    CancellationSucceeded = 12,
    CancellationFailed = 13,
}
```

**Query Handler**:
- **VerifyTransactionQueryHandler** (`src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`):
  - Handles payment verification requests
  - Lines 103-118: BehPardakht settlement pattern (call Settle after Verify success, update status, calculate PredictedSettlementDateTime)
  - Uses `GetPredictedSettlementDateTime()` helper method (lines 202-211) with 23:45 time threshold
  - Updates both Transaction and IPGTransaction entities
  - Saves changes via IAggregateRepository<Transaction>

### Integration Requirements

**Files to Modify**:
1. **`src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`**:
   - Implement Settle() method (currently NotImplementedException at line 147-150)
   - Add settlement-specific error handler (SettleErrorHandler)
   - Add settlement status code arrays
   - Follow existing Verify() pattern for HTTP call

2. **`src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`**:
   - Add Asan Pardakht settlement logic after verification success (similar to BehPardakht pattern lines 103-118)
   - Add provider type check: `if (providerType == ProviderType.AsanPardakht)`
   - Call settlement API, update IPG_Transaction status (9 or 10), calculate PredictedSettlementDateTime
   - Handle settlement-specific date calculation logic (different time threshold 20:40 for failed status)

**New Files to Create**:
- None required - all integration points exist

**Communication Models** (likely need to create):
- `SettleTransactionRequest` and `SettleTransactionResponse` likely exist in `CPG.Domain.SharedKernel.Communication.Ipg.Models.*`
- Need to verify if AsanPardakht-specific request/response models exist

**API Integration Points**:
- **AsanPardakhtProvider.Settle()**: Primary integration point
- **VerifyTransactionQueryHandler.Handle()**: Add Asan Pardakht condition after line 102 (after VerificationSucceeded check)
- **IHttpProvider.PostAsync<>()**: Reuse existing HTTP abstraction
- **GetHeaders()**: Reuse existing authentication header generation

**Data Flow**:
1. VerifyTransactionQueryHandler receives VerifyTransactionQuery
2. Loads PaymentRequest and Transaction from repository
3. Calls IpgProvider.Verify() → success
4. **NEW**: If provider == AsanPardakht, call IpgProvider.Settle()
5. **NEW**: Map Settlement API HTTP status to IPGTransactionStatus (9 or 10)
6. **NEW**: Calculate PredictedSettlementDateTime based on status and current time
7. Update IPGTransaction.Status and Transaction.PredictedSettlementDateTime
8. Save changes to database via repositories

### Implementation Considerations

**Consistency Requirements**:
- Follow existing AsanPardakhtProvider patterns (error handlers, status code arrays, GetDataFromJsonProvider)
- Use same authentication pattern (usr/pwd headers via GetHeaders())
- Match BehPardakht settlement integration pattern in VerifyTransactionQueryHandler
- Use DateTime.Now for all timestamp calculations (consistent with line 60, 99, 204 in VerifyTransactionQueryHandler)
- Maintain existing exception handling patterns (ParseCompanyIpgProviderDataException)

**Potential Conflicts**:
- None identified - settlement is additive functionality
- Asan Pardakht Settle() stub intentionally left for future implementation
- Status codes 9 and 10 already defined in enum

**Refactoring Needs**:
- Consider extracting predicted settlement date calculation logic to separate helper method (one for status 9 with 23:45 threshold, one for status 10 with 20:40 threshold)
- Current `GetPredictedSettlementDateTime()` hardcoded for 23:45 - may need overload or parameter for different thresholds

**Testing Integration**:
- Unit tests should mock IHttpProvider for Settlement API calls
- Test status code mapping (200/474/476 → 9, 471/472/473/475/478 → 10)
- Test predicted date calculation for both status 9 and 10 with various times
- Integration tests for complete verify → settle flow

## Technology Research

### .NET 8.0 / C# 12
**Version**: .NET 8.0 (project already using)
**Research Date**: 2025-10-17
**Documentation Source**: Existing project codebase analysis

**Key Capabilities**:
- HttpClient with IHttpClientFactory for resilient HTTP calls
- Async/await patterns for I/O operations
- Record types for immutable DTOs
- Pattern matching for status code handling

**Best Practices Applied**:
- Dependency injection for HttpClient management
- Repository pattern with EF Core
- CQRS separation (Commands/Queries)
- Clean Architecture layering

**Decision Rationale**: No new technology decisions required - using existing .NET 8.0 stack

### Polly 8.x (Resilience Framework)
**Version**: Polly 8.x (already configured in project)
**Research Date**: 2025-10-17
**Configuration**: `src/CPG.Infrastructure/DependencyInjection.cs` lines 181-186

**Key Capabilities**:
- Automatic retry with exponential backoff
- Circuit breaker for cascading failure prevention
- Timeout policies for long-running requests
- Named policy support per HttpClient

**Current Configuration**:
- `asanpardakhtClient` HttpClient already has Polly policies attached
- Policy retrieved via `policyService.GetHttpPolicy("asanpardakhtClient")`
- Configuration managed centrally in `IPollyPolicyService`

**Decision Rationale**: Reuse existing Polly configuration - no changes needed for Settlement API

## API & Service Research

### Asan Pardakht Settlement API v1
**Documentation Source**: Internal task documentation (docs/taskSettlement.md)
**Research Date**: 2025-10-17
**API Version**: v1
**Base URL**: https://ipgrest.asanpardakht.ir/

**Endpoint**: POST /v1/Settlement

**Authentication Headers**:
- `usr`: Company_IPG.provider_data.User_Name (string)
- `pwd`: Company_IPG.provider_data.Password (string)
- `Accept`: application/json
- `Content-Type`: application/json

**Request Body** (JSON):
```json
{
  "merchantConfigurationId": <numeric>,  // from Company_IPG.provider_data.Merchant_Configuration_Id
  "payGateTranId": <numeric>             // from IPG_Transaction.provider_tracker_id
}
```

**Response Status Codes & Meanings**:
- **200 OK**: Settlement request accepted (first call)
- **474**: Transaction pending for reconciliation (subsequent calls - treat as success)
- **476**: (treat as success)
- **471**: Service error (treat as failure)
- **472**: Unverified transaction - Verify not called before Settlement (treat as failure)
  - Response body: `{"error": {"code": 1029, "message": "Unverified Transaction.", "args": {}}}`
- **473**: Service error (treat as failure)
- **475**: Service error (treat as failure)
- **478**: Service error (treat as failure)

**Response Body Examples**:
1. **Success (200)**: Empty or minimal response
2. **Pending (474)**:
```json
{
  "error": {
    "code": 1026,
    "message": "Transaction Is Pending for Reconcilation.",
    "args": {}
  }
}
```
3. **Unverified (472)**:
```json
{
  "error": {
    "code": 1029,
    "message": "Unverified Transaction.",
    "args": {}
  }
}
```

**Integration Requirements**:
- **Prerequisites**: Transaction must be verified via /v1/Verify before calling Settlement
- **Idempotency**: API handles multiple calls gracefully (returns 474 on subsequent calls)
- **Error Handling**: Map HTTP status to IPGTransactionStatus enum (9 for 200/474/476, 10 for 471/472/473/475/478)
- **Retry Strategy**: Use existing Polly policies attached to asanpardakhtClient

**Constraints**:
- **Sequencing**: Must call Verify before Settlement (472 error if not)
- **Timing**: Settlement should be called immediately after successful verification
- **Rate Limits**: Not specified in documentation (assume Polly policies handle transient failures)

**Decision Rationale**:
- Asan Pardakht Settlement API required by business specification
- Clean REST API with JSON request/response
- Fits existing AsanPardakhtProvider HTTP patterns
- Error codes map well to existing IPGTransactionStatus enum

## Architecture Pattern Research

### Post-Verification Settlement Pattern
**Research Sources**: Existing codebase analysis (BehPardakht implementation)
**Research Date**: 2025-10-17
**Reference Implementation**: `VerifyTransactionQueryHandler.cs` lines 103-118

**Approach**:
The post-verification settlement pattern involves:
1. Verify transaction with IPG provider
2. If verification succeeds, immediately call Settlement API
3. Map settlement response to transaction status
4. Calculate predicted settlement date/time based on settlement status
5. Persist all changes atomically

**Existing Pattern (BehPardakht)**:
```csharp
if (providerType == ProviderType.BehPardakht)
{
    var settlementResult = await ipg.Settle(new SettleTransactionRequest { ... });
    transaction.IPGTransaction.Status = settlementResult.Status;

    if (settlementResult.Status == IPGTransactionStatus.SettlementSucceeded)
    {
        transaction.PredictedSettlementDateTime = GetPredictedSettlementDateTime();
    }
}
```

**Benefits**:
- Immediate settlement request after verification reduces settlement delays
- Atomic transaction updates ensure data consistency
- Existing pattern proven with BehPardakht provider
- Centralized settlement logic in VerifyTransactionQueryHandler

**Drawbacks**:
- Couples verification and settlement in same transaction
- Extends verification API response time (additional HTTP call)
- Settlement failures don't block verification success

**Implementation Considerations**:
- Must handle settlement errors gracefully (don't fail verification)
- Need separate predicted date calculation for failed settlements (different time threshold)
- Settlement call within existing database transaction

**Decision Rationale**: Follow proven BehPardakht pattern - minimizes code changes and maintains architectural consistency

### Status Code Mapping Pattern
**Research Sources**: Existing AsanPardakhtProvider error handlers
**Research Date**: 2025-10-17

**Approach**:
Use status code arrays and pattern matching to map HTTP responses to domain enums:
```csharp
private short[] SettlementSucceededCodes = [200, 474, 476];
private short[] SettlementFailedCodes = [471, 472, 473, 475, 478];

private async Task<TResponse?> SettleErrorHandler<...>(...)
{
    return await Task.FromResult(statusCode switch
    {
        _ when statusCode.IsIn(SettlementSucceededCodes) =>
            new SettleTransactionResponse { Status = IPGTransactionStatus.SettlementSucceeded },
        _ when statusCode.IsIn(SettlementFailedCodes) =>
            new SettleTransactionResponse { Status = IPGTransactionStatus.SettlementFailed },
        _ => new SettleTransactionResponse { Status = IPGTransactionStatus.SettlementFailed },
    });
}
```

**Benefits**:
- Clear, testable status code mappings
- Consistent with existing AsanPardakhtProvider patterns
- Easy to extend if new status codes added
- Default fallback behavior for unknown codes

**Decision Rationale**: Matches existing provider error handling patterns

## Research-Informed Recommendations

### Primary Technology Choices
- **HTTP Client**: Use existing `asanpardakhtClient` with Polly policies - already configured and proven
- **Error Handling**: Follow AsanPardakhtProvider status code array pattern - consistent with existing code
- **DateTime Handling**: Use DateTime.Now - consistent with existing VerifyTransactionQueryHandler
- **Request/Response Models**: Reuse existing SettleTransactionRequest/Response if available, otherwise create minimal DTOs

### Architecture Approach
**Follow BehPardakht Settlement Pattern**:
1. Add `if (providerType == ProviderType.AsanPardakht)` block in VerifyTransactionQueryHandler after verification success (line ~119)
2. Call `ipg.Settle()` with SettleTransactionRequest
3. Update `transaction.IPGTransaction.Status` based on settlement result
4. Calculate `transaction.PredictedSettlementDateTime` using status-specific logic:
   - Status 9 (SettlementSucceeded): Use 23:45 threshold (existing GetPredictedSettlementDateTime)
   - Status 10 (SettlementFailed): Use 20:40 threshold (new helper method or overload)

**AsanPardakhtProvider.Settle() Implementation**:
1. Add status code arrays: `SettlementSucceededCodes`, `SettlementFailedCodes`
2. Implement Settle() method following Verify() pattern
3. Add SettleErrorHandler for status code mapping
4. Use existing GetHeaders() and GetDataFromJsonProvider() methods

### Key Constraints Identified
- **API Sequencing**: Settlement must be called after Verify (472 error otherwise) - handled naturally by placing in VerifyTransactionQueryHandler
- **Time Threshold Differences**: Status 9 uses 23:45, Status 10 uses 20:40 - need two calculation paths
- **Predicted Date Calculation**: Current GetPredictedSettlementDateTime() hardcoded for 23:45 - refactoring needed for reusability
- **Atomic Updates**: Settlement, status update, and predicted date calculation must be in same database transaction
- **Error Resilience**: Polly policies handle transient failures automatically

### Implementation Priorities
1. **CRITICAL**: Implement AsanPardakhtProvider.Settle() method
   - Add status code arrays
   - Implement HTTP POST to /v1/Settlement
   - Add SettleErrorHandler for status mapping
   - Follow existing Verify() pattern exactly

2. **CRITICAL**: Add Asan Pardakht settlement logic to VerifyTransactionQueryHandler
   - Add provider type check after line 118
   - Call Settle() method
   - Update IPGTransaction.Status
   - Calculate PredictedSettlementDateTime with correct threshold

3. **HIGH**: Create predicted date calculation helper for status 10 (20:40 threshold)
   - Extract or overload existing GetPredictedSettlementDateTime()
   - Support both 23:45 and 20:40 thresholds

4. **MEDIUM**: Add comprehensive error logging
   - Log settlement API calls and responses
   - Log status mapping decisions
   - Log predicted date calculations

5. **DEFERRED**: Consider async settlement via background job
   - Current approach (sync in VerifyTransactionQueryHandler) matches BehPardakht
   - Future optimization if verification response time becomes issue

---

**Next Phase**: This research provides the technical knowledge foundation for architectural planning in Tech.md.