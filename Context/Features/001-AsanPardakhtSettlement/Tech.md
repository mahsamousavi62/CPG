# Technical Architecture: Asan Pardakht Settlement Service Integration

**Created**: 2025-10-17
**Status**: Technical Plan
**Prerequisites**: Completed business specification (Spec.md) and technical research (Research.md)

## System Overview

### High-Level Architecture

The Asan Pard

akht Settlement integration follows the existing Clean Architecture pattern used throughout CPG. The implementation extends two existing components:

1. **AsanPardakhtProvider** (Infrastructure Layer): Implements the Settle() method to call the Settlement REST API
2. **VerifyTransactionQueryHandler** (Persistence Layer): Adds settlement logic after successful transaction verification

```
┌─────────────────────────────────────────────────────────┐
│ Application Layer (CQRS)                                │
│ ┌───────────────────────────────────────────────────┐   │
│ │ VerifyTransactionQuery                            │   │
│ └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ Infrastructure.Persistence Layer                        │
│ ┌───────────────────────────────────────────────────┐   │
│ │ VerifyTransactionQueryHandler                     │   │
│ │  ├─ Call IpgProvider.Verify()                     │   │
│ │  ├─ [NEW] Check if AsanPardakht provider          │   │
│ │  ├─ [NEW] Call IpgProvider.Settle()               │   │
│ │  ├─ [NEW] Update IPGTransaction.Status (9 or 10)  │   │
│ │  └─ [NEW] Calculate PredictedSettlementDateTime   │   │
│ └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ Infrastructure Layer (Providers)                        │
│ ┌───────────────────────────────────────────────────┐   │
│ │ AsanPardakhtProvider : IIpgProvider               │   │
│ │  └─ [NEW] Settle(SettleTransactionRequest)        │   │
│ │     ├─ Extract credentials from ProviderData      │   │
│ │     ├─ POST https://ipgrest.asanpardakht.ir/v1/   │   │
│ │     │   Settlement                                 │   │
│ │     ├─ Map HTTP status to IPGTransactionStatus    │   │
│ │     └─ Return SettleTransactionResponse           │   │
│ └───────────────────────────────────────────────────┘   │
│                       ↓                                 │
│ ┌───────────────────────────────────────────────────┐   │
│ │ IHttpProvider (with Polly policies)               │   │
│ │  └─ asanpardakhtClient HttpClient                 │   │
│ └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│ Domain Layer                                            │
│ ┌───────────────────────────────────────────────────┐   │
│ │ IPGTransaction (Status updated)                   │   │
│ │ Transaction (PredictedSettlementDateTime updated) │   │
│ └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### Core Components

- **AsanPardakhtProvider** (`src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`):
  - Implements IIpgProvider.Settle() method (currently NotImplementedException)
  - Handles Settlement API HTTP communication
  - Maps HTTP status codes to domain status enums

- **VerifyTransactionQueryHandler** (`src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`):
  - Orchestrates verify → settle workflow for Asan Pardakht
  - Updates IPGTransaction and Transaction entities
  - Calculates predicted settlement dates

- **IPGTransaction & Transaction Entities** (`src/CPG.Domain/AggregateModels/TransactionAggregate/`):
  - Domain models updated with settlement status and predicted dates
  - No structural changes needed - existing properties sufficient

### Data Flow

1. External application calls VerifyTransactionQuery (via API endpoint)
2. VerifyTransactionQueryHandler loads PaymentRequest and Transaction from database
3. Handler calls AsanPardakhtProvider.Verify() → returns success
4. **NEW**: Handler checks provider type == AsanPardakht
5. **NEW**: Handler calls AsanPardakhtProvider.Settle() with transaction data
6. **NEW**: Settle() extracts credentials from CompanyIPG.ProviderData
7. **NEW**: Settle() sends POST to https://ipgrest.asanpardakht.ir/v1/Settlement
8. **NEW**: Polly policies handle retries/circuit breaking automatically
9. **NEW**: Settle() maps HTTP status (200/474/476 → status 9, others → status 10)
10. **NEW**: Handler updates IPGTransaction.Status based on settlement result
11. **NEW**: Handler calculates Transaction.PredictedSettlementDateTime (status-specific logic)
12. Handler saves all changes to database via repositories
13. Response returned to caller with updated transaction info

## .NET Clean Architecture Implementation Details

### Infrastructure Layer - AsanPardakhtProvider

**File**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`

**New Status Code Arrays**:
```csharp
private short[] SettlementSucceededCodes = [200, 474, 476];
private short[] SettlementFailedCodes = [471, 472, 473, 475, 478];
```

**Settle() Method Implementation**:
```csharp
public async Task<SettleTransactionResponse> Settle(SettleTransactionRequest request)
{
    GetDataFromJsonProvider(request.ProviderData);
    var headers = GetHeaders();

    var response = await httpProvider.PostAsync<SettleTransactionRequest, SettleTransactionResponse,
                                                AsanPardakhtResponseBase, dynamic>(
        new HttpProviderRequest<dynamic>
        {
            Body = new AsanPardakhtSettleRequest
            {
                PayGateTranId = request.ProviderTrackerId,
                MerchantConfigurationId = merchantConfigurationId
            },
            BaseAddress = "https://ipgrest.asanpardakht.ir/",
            Uri = "v1/Settlement",
            HeaderParameters = headers,
            Provider = Enums.ProviderTypeInLog.AsanPardakht,
            Service = Enums.ServiceType.AsanPardakhtSettle,
        }, request, SettleErrorHandler, (string stringResponse) =>
        {
            // Empty or minimal response expected for 200
            if (string.IsNullOrEmpty(stringResponse))
                return new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementSucceeded };

            return System.Text.Json.JsonSerializer.Deserialize<SettleTransactionResponse>(stringResponse);
        });

    return response;
}
```

**SettleErrorHandler Implementation**:
```csharp
private async Task<TResponse?> SettleErrorHandler<TBaseRequest, TResponse, TError>(
    TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : SettleTransactionResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : SettleTransactionRequest
{
    return await Task.FromResult(statusCode switch
    {
        _ when statusCode.IsIn(SettlementSucceededCodes) =>
            new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementSucceeded } as TResponse,
        _ when statusCode.IsIn(SettlementFailedCodes) =>
            new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementFailed } as TResponse,
        _ =>
            new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementFailed } as TResponse,
    });
}
```

**Request/Response Models**:
- **AsanPardakhtSettleRequest**: Simple DTO with `MerchantConfigurationId` and `PayGateTranId` properties
- **SettleTransactionResponse**: Contains `Status` (IPGTransactionStatus enum)
- These likely already exist in `CPG.Domain.SharedKernel.Communication.Ipg.Models.*` namespace

**Decision Rationale**:
- Follows existing AsanPardakhtProvider patterns exactly (Verify, GetPaymentToken, GetTransactionResult)
- Reuses GetDataFromJsonProvider() and GetHeaders() methods
- Uses existing IHttpProvider abstraction with Polly policies
- Status code mapping pattern matches VerifyErrorHandler

### Persistence Layer - VerifyTransactionQueryHandler

**File**: `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`

**Integration Point**: After line 118 (after BehPardakht settlement block)

**New Code Block**:
```csharp
else if (providerType == ProviderType.AsanPardakht)
{
    var settlementResult = await ipg.Settle(new SettleTransactionRequest
    {
        ProviderData = transaction.IPGTransaction.CompanyIPG.ProviderData,
        ProviderTrackerId = transaction.IPGTransaction.ProviderTrackerId,
    });

    transaction.IPGTransaction.Status = settlementResult.Status;

    // Calculate predicted settlement date based on status
    if (settlementResult.Status == IPGTransactionStatus.SettlementSucceeded)
    {
        // Use existing GetPredictedSettlementDateTime (23:45 threshold)
        transaction.PredictedSettlementDateTime = GetPredictedSettlementDateTime();
    }
    else if (settlementResult.Status == IPGTransactionStatus.SettlementFailed)
    {
        // Use new method with 20:40 threshold
        transaction.PredictedSettlementDateTime = GetPredictedSettlementDateTimeForFailedSettlement();
    }
}
```

**New Helper Method**:
```csharp
private static DateTime GetPredictedSettlementDateTimeForFailedSettlement()
{
    var currentDateTime = DateTime.Now;
    var timeMargin = new TimeOnly(20, 40);
    var currentTime = new TimeOnly(currentDateTime.Hour, currentDateTime.Minute);
    var date = currentTime < timeMargin ?
        new DateTime(currentDateTime.AddDays(1).Year, currentDateTime.AddDays(1).Month, currentDateTime.AddDays(1).Day, 7, 0, 0) :
        new DateTime(currentDateTime.AddDays(2).Year, currentDateTime.AddDays(2).Month, currentDateTime.AddDays(2).Day, 7, 0, 0);
    return date;
}
```

**Decision Rationale**:
- Mirrors existing BehPardakht pattern (lines 103-118) for consistency
- Status-specific predicted date calculation (different thresholds for success vs. failure)
- Reuses existing GetPredictedSettlementDateTime() for successful settlements
- New helper method for failed settlements (20:40 threshold per specification)
- Settlement within same database transaction ensures atomicity

### Domain Layer - No Changes Required

**Entities**: `src/CPG.Domain/AggregateModels/TransactionAggregate/`

**IPGTransaction.cs**:
- Existing `Status` property (IPGTransactionStatus enum) - supports status 9 and 10
- Existing `ModificationDate` (from AuditableEntity) - automatically updated by interceptor
- No structural changes needed

**Transaction.cs**:
- Existing `PredictedSettlementDateTime` property (DateTime?) - will be populated
- Existing `ModificationDate` (from AuditableEntity) - automatically updated by interceptor
- No structural changes needed

**Enum** (Already Defined in `src/CPG.Domain/SharedKernel/Enums.cs`):
```csharp
public enum IPGTransactionStatus : byte
{
    // ... existing values
    SettlementSucceeded = 9,
    SettlementFailed = 10,
    // ...
}
```

**Decision Rationale**:
- Existing domain models already support settlement functionality
- Status codes 9 and 10 were pre-defined for settlement scenarios
- AuditableEntity interceptor handles modification timestamps automatically
- No breaking changes to domain contracts

### Communication Models

**Namespace**: `CPG.Domain.SharedKernel.Communication.Ipg.Models.*`

**Required Models** (likely already exist, verify during implementation):
- **SettleTransactionRequest**: Base request model
  - `ProviderData` (string)
  - `ProviderTrackerId` (string)

- **SettleTransactionResponse**: Response model
  - `Status` (IPGTransactionStatus enum)
  - `StatusCode` (short)

- **AsanPardakhtSettleRequest**: Provider-specific request
  - `MerchantConfigurationId` (int)
  - `PayGateTranId` (long)

If these don't exist, they follow existing patterns in `CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify/` namespace.

### HTTP Client Configuration

**File**: `src/CPG.Infrastructure/DependencyInjection.cs` (lines 181-186)

**Existing Configuration** (no changes needed):
```csharp
services.AddHttpClient("asanpardakhtClient", c =>
{
    c.Timeout = TimeSpan.FromSeconds(30);
    // ... other configuration
})
.AddPolicyHandler(policyService.GetHttpPolicy("asanpardakhtClient"));
```

**Polly Policies** (already configured):
- Retry with exponential backoff
- Circuit breaker
- Timeout protection
- Complete request/response logging via PollyLoggingHandler

**Decision Rationale**:
- Existing asanpardakhtClient HttpClient already has all required policies
- Settlement API will benefit from same resilience patterns as Verify API
- No infrastructure changes needed

## Implementation Complexity Assessment

### Technical Complexity Assessment

**Complexity Level**: **Moderate**

**Reasoning**:
- Extends two existing files (AsanPardakhtProvider, VerifyTransactionQueryHandler)
- Follows established patterns (BehPardakht settlement as reference)
- No new infrastructure, database migrations, or domain model changes
- Main complexity: Status-specific predicted date calculation logic

**Implementation Challenges**:

1. **Setup and Infrastructure**: ✅ **Low Complexity**
   - HttpClient already configured with Polly policies
   - No new dependencies or configuration needed
   - Authentication pattern already established

2. **Core Implementation**: ⚠️ **Moderate Complexity**
   - AsanPardakhtProvider.Settle(): Follow Verify() pattern exactly (straightforward)
   - SettleErrorHandler: Simple status code mapping (straightforward)
   - VerifyTransactionQueryHandler changes: Mirror BehPardakht pattern (straightforward)
   - Predicted date calculation: Two separate helper methods needed (moderate - requires careful testing of time thresholds)

3. **Integration Points**: ✅ **Low Complexity**
   - IHttpProvider abstraction handles HTTP communication
   - IpgFactory provides correct provider instance
   - Repository pattern handles database persistence
   - All integration points already proven

4. **Testing Requirements**: ⚠️ **Moderate Complexity**
   - Unit tests for AsanPardakhtProvider.Settle() with mocked IHttpProvider
   - Unit tests for SettleErrorHandler status mapping (all status codes)
   - Unit tests for predicted date calculation (edge cases: 23:45, 20:40, before/after midnight)
   - Integration tests for complete verify → settle flow
   - Need to test both successful and failed settlement paths

### Risk Assessment

**High Risk Areas**:
1. **Time Threshold Edge Cases**:
   - Risk: Incorrect predicted date calculation at exactly 23:45:00 or 20:40:00
   - Mitigation: Comprehensive unit tests for boundary conditions; use < operator per specification

2. **Settlement API HTTP Status Mapping**:
   - Risk: Misinterpreting status codes (e.g., 474 as failure instead of success)
   - Mitigation: Follow task documentation exactly; add integration tests with mock API responses

3. **Provider Type Check**:
   - Risk: Settlement called for wrong provider (not AsanPardakht)
   - Mitigation: Provider type check already proven with BehPardakht pattern; add defensive logging

**Medium Risk Areas**:
1. **Database Transaction Atomicity**:
   - Risk: Settlement succeeds but database save fails
   - Mitigation: Existing repository pattern with UnitOfWork ensures atomicity; proven with BehPardakht

2. **Polly Policy Behavior**:
   - Risk: Retry logic causes duplicate settlement requests (474 responses)
   - Mitigation: Asan Pardakht API is idempotent (returns 474 on duplicate calls); Polly configured correctly

**Unknowns Requiring Verification**:
- Do SettleTransactionRequest/Response models already exist? (Check `CPG.Domain.SharedKernel.Communication.Ipg.Models.*`)
- Does AsanPardakhtSettleRequest model exist? (Check `CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht.*`)
- What is the enum value for `Enums.ServiceType.AsanPardakhtSettle`? (May need to add if not exists)

### Dependency Analysis

**External Dependencies**:
- None - all required packages already in project

**Internal Dependencies**:

**Existing Code Modifications**:
1. `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`:
   - Replace NotImplementedException in Settle() method (line 147-150)
   - Add SettlementSucceededCodes and SettlementFailedCodes arrays
   - Add SettleErrorHandler method

2. `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`:
   - Add `else if (providerType == ProviderType.AsanPardakht)` block after line 118
   - Add GetPredictedSettlementDateTimeForFailedSettlement() helper method

**New Shared Components**:
- Potentially AsanPardakhtSettleRequest model (if doesn't exist)
- Potentially SettleTransactionRequest/Response models (if don't exist)

**Breaking Changes**: None

### Quality Assurance Requirements

**Testing Strategy**:

**Unit Tests**:
- AsanPardakhtProvider.Settle(): Mock IHttpProvider, verify correct HTTP request construction
- SettleErrorHandler: Test all status codes (200, 474, 476, 471, 472, 473, 475, 478, unknown)
- GetPredictedSettlementDateTime: Test times before/at/after 23:45 threshold
- GetPredictedSettlementDateTimeForFailedSettlement: Test times before/at/after 20:40 threshold
- Date calculation edge cases: just before midnight, DST transitions

**Integration Tests**:
- End-to-end verify → settle flow for Asan Pardakht provider
- Verify settlement NOT called for other providers (BehPardakht, Sep, etc.)
- Database persistence of IPGTransaction.Status and Transaction.PredictedSettlementDateTime
- Polly retry behavior with simulated transient failures

**Manual Testing Checklist**:
- [ ] Settlement API call with valid credentials returns 200
- [ ] Subsequent settlement call returns 474 (idempotency)
- [ ] Settlement before Verify returns 472 error
- [ ] Status 9 predicted date calculated correctly (23:45 threshold)
- [ ] Status 10 predicted date calculated correctly (20:40 threshold)
- [ ] Polly retries on 5xx errors
- [ ] Polly circuit breaker opens after repeated failures
- [ ] Complete request/response logged on timeout

**Validation Requirements**:
- **Clean Architecture Validation**: All changes stay within appropriate layers (no domain contamination)
- **CQRS Compliance**: Settlement triggered from query handler (matches existing BehPardakht pattern)
- **Performance Testing**: Verify API response time impact (additional HTTP call ~100-200ms)

## Technical Clarifications

### Areas Requiring Resolution

No blocking technical uncertainties identified. Implementation path is clear based on existing patterns.

### Verification Requirements

**Before Implementation**:
1. Verify SettleTransactionRequest/Response models exist in `CPG.Domain.SharedKernel.Communication.Ipg.Models.*`
2. Verify AsanPardakhtSettleRequest model exists in `CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht.*`
3. Check if `Enums.ServiceType.AsanPardakhtSettle` enum value exists
4. Confirm `Enums.ProviderType.AsanPardakht` enum value exists and is used correctly

**During Implementation**:
1. Review Polly logging output format to ensure settlement calls are logged appropriately
2. Verify AuditableEntityInterceptor updates ModificationDate automatically
3. Test with actual Asan Pardakht sandbox credentials (if available)

---

**Next Phase**: After this technical architecture is approved, proceed to `/ctxk:plan:3-steps` for implementation task breakdown and development planning.
