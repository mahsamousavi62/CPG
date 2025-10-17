# Implementation Steps: Asan Pardakht Settlement Service Integration

**Created**: 2025-10-17
**Status**: Implementation Plan
**Prerequisites**: Completed business specification (Spec.md), technical research (Research.md), and technical architecture (Tech.md)

## Implementation Phases *(mandatory)*

### Phase 1: Setup & Configuration
*Foundation tasks - verify existing infrastructure and communication models*

- [x] **S001** Verify communication models exist
  - **Path**: `src/CPG.Domain/SharedKernel/Communication/Ipg/Models/`
  - **Dependencies**: None
  - **Action**: Check if SettleTransactionRequest, SettleTransactionResponse, AsanPardakhtSettleRequest models exist
  - **Notes**: ✅ SettleTransactionRequest and SettleTransactionResponse exist. Created AsanPardakhtSettleRequest model with MerchantConfigurationId and PayGateTranId properties following AsanPardakhtVerifyRequest pattern

- [x] **S002** Verify ServiceType enum includes AsanPardakhtSettle
  - **Path**: `src/CPG.Domain/SharedKernel/Enums.cs`
  - **Dependencies**: None
  - **Action**: Check ServiceType enum for AsanPardakhtSettle value; add if missing
  - **Notes**: ✅ Added AsanPardakhtSettle = 103 to ServiceType enum (adjusted FileStorage to 104, Database to 105)

**🏁 MILESTONE: Prerequisites Verified**
*Use Task tool with commit-changes agent to commit: "Verify AsanPardakht settlement prerequisites - models and enums"*

### Phase 2: AsanPardakhtProvider Settlement Implementation
*Core provider layer implementation with TDD approach*

#### Test-First Implementation
- [x] **S003** [P] Create unit tests for SettleErrorHandler status code mapping
  - **Path**: `tests/CPG.Domain.Tests.Unit/Providers/Ipg/AsanPardakhtProviderTests.cs` (create if missing)
  - **Dependencies**: S001 (models exist)
  - **Action**: Test all status codes: 200, 474, 476 → SettlementSucceeded; 471, 472, 473, 475, 478 → SettlementFailed; unknown → SettlementFailed
  - **Notes**: ✅ Created comprehensive test file with Theory tests for all status code mappings

- [x] **S004** [P] Create unit tests for AsanPardakhtProvider.Settle() method
  - **Path**: `tests/CPG.Domain.Tests.Unit/Providers/Ipg/AsanPardakhtProviderTests.cs`
  - **Dependencies**: S001 (models exist)
  - **Action**: Mock IHttpProvider; verify correct request body (MerchantConfigurationId, PayGateTranId); verify headers (usr, pwd); test success/failure paths
  - **Notes**: ✅ Added tests for HttpProvider calls, header validation, body construction, empty response handling, and invalid data exceptions

#### Provider Implementation
- [x] **S005** Add settlement status code arrays to AsanPardakhtProvider
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs` (private fields section)
  - **Dependencies**: None
  - **Action**: Add `private short[] SettlementSucceededCodes = [200, 474, 476];` and `private short[] SettlementFailedCodes = [471, 472, 473, 475, 478];`
  - **Notes**: ✅ Added settlement status code arrays following existing pattern

- [x] **S006** Implement SettleErrorHandler method in AsanPardakhtProvider
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs` (after VerifyErrorHandler)
  - **Dependencies**: S005 (status code arrays), S001 (models)
  - **Action**: Implement generic error handler using pattern matching with SettlementSucceededCodes/FailedCodes arrays
  - **Notes**: ✅ Implemented SettleErrorHandler with pattern matching, maps to SettlementSucceeded (200/474/476) or SettlementFailed (471/472/473/475/478 and default)

- [x] **S007** Implement AsanPardakhtProvider.Settle() method
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs` (replace NotImplementedException at line 147-150)
  - **Dependencies**: S006 (SettleErrorHandler), S001 (models), S002 (ServiceType enum)
  - **Action**: Call GetDataFromJsonProvider; create AsanPardakhtSettleRequest; call httpProvider.PostAsync with "v1/Settlement" endpoint, SettleErrorHandler, and success response deserializer
  - **Notes**: ✅ Implemented Settle() method following Verify() pattern: extracts credentials, builds AsanPardakhtSettleRequest with PayGateTranId and MerchantConfigurationId, calls v1/Settlement endpoint, handles empty 200 response

**🏁 MILESTONE: Provider Implementation Complete**
*Use Task tool with commit-changes agent to commit: "Implement AsanPardakht settlement provider integration"*

### Phase 3: VerifyTransactionQueryHandler Integration
*Query handler orchestration and date calculation logic*

#### Predicted Date Calculation Testing
- [ ] **S008** [P] Create unit tests for GetPredictedSettlementDateTimeForFailedSettlement helper
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/QueryHandlers/Ipg/VerifyTransactionQueryHandlerTests.cs` (create if missing)
  - **Dependencies**: None
  - **Action**: Test time < 20:40 returns current date + 1 day at 07:00; time >= 20:40 returns current date + 2 days at 07:00; test edge cases (20:40:00, 23:59:59, 00:00:00)
  - **Notes**: Use TimeProvider pattern for testable DateTime.Now; verify timezone consistency

- [ ] **S009** [P] Create integration tests for complete verify → settle flow
  - **Path**: `tests/CPG.Integration.Tests/Ipg/AsanPardakhtSettlementFlowTests.cs` (create if missing)
  - **Dependencies**: S007 (Settle() implemented)
  - **Action**: Test end-to-end: verify success → settle called → IPGTransaction.Status updated → PredictedSettlementDateTime calculated; test AsanPardakht vs. other providers
  - **Notes**: Mock Settlement API HTTP responses; verify database persistence

#### Query Handler Implementation
- [x] **S010** Add GetPredictedSettlementDateTimeForFailedSettlement helper method
  - **Path**: `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs` (after GetPredictedSettlementDateTime method)
  - **Dependencies**: None
  - **Action**: Implement static method with 20:40 time threshold logic (current time < 20:40: +1 day at 07:00, else +2 days at 07:00)
  - **Notes**: ✅ Implemented helper method with TimeOnly(20, 40) threshold, mirrors GetPredictedSettlementDateTime() pattern

- [x] **S011** Add AsanPardakht settlement logic to VerifyTransactionQueryHandler
  - **Path**: `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs` (after line 118, after BehPardakht settlement block)
  - **Dependencies**: S007 (Settle() method), S010 (helper method)
  - **Action**: Add `else if (providerType == ProviderType.AsanPardakht)` block; call ipg.Settle() with SettleTransactionRequest; update transaction.IPGTransaction.Status; calculate transaction.PredictedSettlementDateTime based on status (9 → 23:45 threshold, 10 → 20:40 threshold)
  - **Notes**: ✅ Added AsanPardakht settlement block after line 118: calls Settle() with ProviderData and ProviderTrackerId, updates IPGTransaction.Status, calculates PredictedSettlementDateTime using GetPredictedSettlementDateTime() for success or GetPredictedSettlementDateTimeForFailedSettlement() for failure

**🏁 MILESTONE: Settlement Orchestration Complete**
*Use Task tool with commit-changes agent to commit: "Integrate AsanPardakht settlement into verification workflow"*

### Phase 4: Automated Build & Test Validation
*Compile-time validation and automated test execution*

- [ ] **S012** Build solution and verify no compilation errors
  - **Path**: Root solution `CPG.sln`
  - **Dependencies**: S001, S002, S005, S006, S007, S010, S011 (all implementation tasks)
  - **Action**: Use Task tool with build-project agent - run `dotnet build CPG.sln --configuration Release`
  - **Notes**: Verify all new code compiles; resolve any missing using statements or type references

- [ ] **S013** [P] Run AsanPardakhtProvider unit tests
  - **Path**: `tests/CPG.Domain.Tests.Unit/Providers/Ipg/AsanPardakhtProviderTests.cs`
  - **Dependencies**: S012 (build success), S003, S004 (tests created)
  - **Action**: Use Task tool with run-specific-test agent - run `dotnet test --filter FullyQualifiedName~AsanPardakhtProviderTests`
  - **Notes**: All status code mapping tests must pass; Settle() method tests must verify correct HTTP request construction

- [ ] **S014** [P] Run VerifyTransactionQueryHandler unit tests
  - **Path**: `tests/CPG.Infrastructure.Tests.Unit/QueryHandlers/Ipg/VerifyTransactionQueryHandlerTests.cs`
  - **Dependencies**: S012 (build success), S008 (tests created)
  - **Action**: Use Task tool with run-specific-test agent - run tests for GetPredictedSettlementDateTimeForFailedSettlement method
  - **Notes**: Verify time threshold logic (20:40 boundary), edge cases (midnight, exact threshold time)

- [ ] **S015** Run integration tests for settlement flow
  - **Path**: `tests/CPG.Integration.Tests/Ipg/AsanPardakhtSettlementFlowTests.cs`
  - **Dependencies**: S012 (build success), S009 (tests created)
  - **Action**: Use Task tool with run-specific-test agent - run end-to-end settlement workflow tests
  - **Notes**: Verify verify → settle → status update → predicted date calculation flow; confirm provider type filtering works

**🏁 MILESTONE: Automated Testing Complete**
*Use Task tool with commit-changes agent to commit: "Complete AsanPardakht settlement automated testing"*

### Phase 5: Code Quality & Standards Validation
*Automated code quality checks and best practices validation*

- [ ] **S016** [P] Run full test suite for regression validation
  - **Path**: All test projects in solution
  - **Dependencies**: S012, S013, S014, S015 (all tests passing)
  - **Action**: Use Task tool with run-test-suite agent - run `dotnet test CPG.sln --configuration Release`
  - **Notes**: Ensure no existing tests broken by settlement changes; all tests must pass

- [ ] **S017** [P] Validate error handling patterns
  - **Path**: `src/CPG.Infrastructure/Providers/Ipg/AsanPardakhtProvider.cs`, `src/CPG.Infrastructure.Persistence/QueryHandlers/Ipg/VerifyTransactionQueryHandler.cs`
  - **Dependencies**: S007, S011 (implementation complete)
  - **Action**: Review exception handling in Settle() method and settlement orchestration; verify Polly policies handle transient failures
  - **Notes**: Confirm ParseCompanyIpgProviderDataException thrown on invalid ProviderData; verify SettleErrorHandler maps all status codes; check settlement errors don't fail verification

- [ ] **S018** Validate logging completeness
  - **Path**: AsanPardakhtProvider.Settle() and VerifyTransactionQueryHandler settlement block
  - **Dependencies**: S007, S011 (implementation complete)
  - **Action**: Verify PollyLoggingHandler logs settlement API calls; confirm ServiceType.AsanPardakhtSettle used for provider logging
  - **Notes**: Check logs include: settlement request, response status, retry attempts, predicted date calculations

**🏁 MILESTONE: Quality Validation Complete**
*Use Task tool with commit-changes agent to commit: "Complete AsanPardakht settlement quality validation"*

### Phase 6: Manual API Testing & Validation
*Tasks requiring interaction with live/sandbox Asan Pardakht API*

- [ ] **S019** Manual happy path settlement testing (sandbox environment)
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 MANUAL API TESTING REQUIRED (Sandbox)
  ═══════════════════════════════════════════════════
  ║
  ║ Prerequisites:
  ║ • Asan Pardakht sandbox credentials configured in Company_IPG.provider_data
  ║ • API running locally or deployed to development environment
  ║
  ║ Test Steps:
  ║ 1. Create a test payment request for Asan Pardakht provider
  ║ 2. Complete payment flow (GetPaymentToken → redirect → GetTransactionResult)
  ║ 3. Call Verify API endpoint for the transaction
  ║ 4. Verify settlement API called automatically (check logs)
  ║ 5. Verify IPG_Transaction.status = 9 (SettlementSucceeded)
  ║ 6. Verify Transaction.predicted_settlement_date_time calculated correctly
  ║    (if before 23:45: current date + 1 day at 07:00, else + 2 days)
  ║ 7. Call Verify API again for same transaction (idempotency test)
  ║ 8. Verify settlement returns HTTP 474 (pending reconciliation)
  ║ 9. Verify IPG_Transaction.status remains 9
  ║
  ║ Success Criteria:
  ║ • HTTP 200 on first settlement call
  ║ • HTTP 474 on subsequent calls (idempotent behavior)
  ║ • Status correctly set to 9
  ║ • Predicted date calculated based on 23:45 threshold
  ║ • Settlement logged in Serilog/Elasticsearch
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

- [ ] **S020** Manual error scenario testing (settlement before verify)
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 MANUAL ERROR SCENARIO TESTING REQUIRED
  ═══════════════════════════════════════════════════
  ║
  ║ Test Edge Case: Settlement called before Verify
  ║
  ║ Test Steps:
  ║ 1. Create payment request, complete payment flow
  ║ 2. DO NOT call Verify endpoint
  ║ 3. Manually call Settlement API directly (via Postman/curl)
  ║ 4. Verify API returns HTTP 472 (Unverified Transaction)
  ║ 5. Verify error response: {"error": {"code": 1029, "message": "Unverified Transaction."}}
  ║
  ║ Expected: Settlement fails gracefully with proper error code
  ║
  ║ Reply "✅ Passed" or "❌ Issues: [description]"
  ```

- [ ] **S021** Manual predicted date calculation validation (time boundaries)
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 TIME THRESHOLD VALIDATION REQUIRED
  ═══════════════════════════════════════════════════
  ║
  ║ Test Scenarios:
  ║
  ║ 1. Successful settlement BEFORE 23:45:
  ║    • Run verify/settle before 23:45
  ║    • Verify predicted_settlement_date_time = (current date + 1 day) at 07:00
  ║
  ║ 2. Successful settlement AFTER 23:45:
  ║    • Run verify/settle after 23:45
  ║    • Verify predicted_settlement_date_time = (current date + 2 days) at 07:00
  ║
  ║ 3. Failed settlement BEFORE 20:40 (simulate with invalid data):
  ║    • Trigger settlement failure before 20:40
  ║    • Verify predicted_settlement_date_time = (current date + 1 day) at 07:00
  ║    • Verify IPG_Transaction.status = 10 (SettlementFailed)
  ║
  ║ 4. Failed settlement AFTER 20:40:
  ║    • Trigger settlement failure after 20:40
  ║    • Verify predicted_settlement_date_time = (current date + 2 days) at 07:00
  ║
  ║ Note: May need to adjust server time or wait for specific times to test
  ║
  ║ Reply "✅ All thresholds validated" or "❌ Issues: [description]"
  ```

- [ ] **S022** Manual Polly retry policy validation
  ```
  ═══════════════════════════════════════════════════
  ║ 🧪 RESILIENCE POLICY TESTING REQUIRED
  ═══════════════════════════════════════════════════
  ║
  ║ Test Polly Retry Behavior:
  ║
  ║ 1. Simulate transient failure (temporarily block settlement API endpoint)
  ║ 2. Call verify endpoint for Asan Pardakht transaction
  ║ 3. Monitor logs for retry attempts (should see 3 retries with exponential backoff)
  ║ 4. Restore API access
  ║ 5. Verify settlement succeeds after retry
  ║
  ║ Test Circuit Breaker:
  ║ 1. Simulate persistent API failure (5+ consecutive failures)
  ║ 2. Verify circuit breaker opens (logs show circuit open state)
  ║ 3. Verify subsequent calls fail fast (no settlement attempted)
  ║ 4. Wait for circuit half-open period
  ║ 5. Verify circuit closes after successful call
  ║
  ║ Logging Validation:
  ║ • Check PollyLoggingHandler logs complete request/response on timeout
  ║ • Verify ServiceType.AsanPardakhtSettle appears in logs
  ║
  ║ Reply "✅ Polly policies working correctly" or "❌ Issues: [description]"
  ```

**🏁 MILESTONE: Manual Testing Complete**
*All API integration scenarios validated with sandbox environment*

### Phase 7: Release Preparation & Documentation
*Final validation and deployment preparation*

- [ ] **S023** [P] Update feature documentation
  - **Path**: `Context/Features/001-AsanPardakhtSettlement/` directory
  - **Dependencies**: All implementation complete
  - **Action**: Mark Spec.md, Research.md, Tech.md as "Completed"; add implementation notes to Research.md (any deviations from plan, lessons learned)
  - **Notes**: Document final status code mappings verified, date calculation thresholds confirmed, Polly policy behavior observed

- [ ] **S024** [P] Update CLAUDE.md with settlement pattern documentation
  - **Path**: `CLAUDE.md` (Resilience and Retry Policies section)
  - **Dependencies**: S011 (implementation complete)
  - **Action**: Add AsanPardakht Settlement API to list of Polly-enabled endpoints; document settlement as part of post-verification workflow
  - **Notes**: Reference BehPardakht and AsanPardakht as examples of settlement integration pattern

- [ ] **S025** Prepare deployment checklist
  ```
  ═══════════════════════════════════════════════════
  ║ 🚀 DEPLOYMENT PREPARATION CHECKLIST
  ═══════════════════════════════════════════════════
  ║
  ║ Pre-Deployment Verification:
  ║ 1. All unit tests passing (S013, S014)
  ║ 2. All integration tests passing (S015)
  ║ 3. Full test suite passing (S016)
  ║ 4. Manual API testing completed with sandbox (S019-S022)
  ║ 5. Code reviewed for Clean Architecture compliance
  ║ 6. Polly policies validated for settlement endpoint
  ║ 7. Logging verified in development environment
  ║
  ║ Configuration Checklist:
  ║ • Verify production Asan Pardakht credentials in Company_IPG.provider_data
  ║ • Confirm ServiceType.AsanPardakhtSettle enum exists in production
  ║ • Validate Polly policy configuration in production appsettings.json
  ║ • Ensure Elasticsearch/Serilog logging configured for settlement events
  ║
  ║ Deployment Pipeline:
  ║ • Merge feature/001-asan-pardakht-settlement → develop
  ║ • Validate build in develop pipeline (azure-pipeline.yaml)
  ║ • Deploy to development environment (cpg/dev/cpg-api-backend)
  ║ • Run smoke tests with Asan Pardakht sandbox
  ║ • Merge develop → stage for staging validation
  ║ • Deploy to staging (cpg/stage/cpg-api-backend)
  ║ • Perform final acceptance testing in staging
  ║ • Merge stage → master for production deployment
  ║
  ║ Reply "✅ Ready for deployment" when all checks passed
  ```

**🏁 MILESTONE: Release Ready**
*Use Task tool with commit-changes agent to commit: "Finalize AsanPardakht settlement - ready for deployment"*

## AI-Assisted Development Time Estimation *(Claude Code + Human Review)*

> **⚠️ ESTIMATION BASIS**: These estimates assume development with Claude Code (AI) executing implementation tasks with human review and guidance. Times reflect AI execution + human review cycles, not manual coding.

### Phase-by-Phase Review Time

**Phase 1: Setup & Configuration** (S001-S002): **30 minutes**
- AI execution: 10 minutes (verify models, check enums)
- Human review: 20 minutes (validate model structure, confirm enum values)

**Phase 2: AsanPardakhtProvider Implementation** (S003-S007): **2-3 hours**
- AI execution: 45 minutes (tests + provider implementation)
- Human review: 60-90 minutes (validate status code mappings, verify HTTP patterns, review error handling)
- Correction cycles: 30 minutes (refine error handlers, adjust request models)

**Phase 3: VerifyTransactionQueryHandler Integration** (S008-S011): **2-2.5 hours**
- AI execution: 40 minutes (tests + query handler changes)
- Human review: 60 minutes (validate date calculation logic, verify provider type check, review BehPardakht pattern consistency)
- Correction cycles: 20-30 minutes (adjust time threshold logic, refine predicted date calculations)

**Phase 4: Automated Build & Test Validation** (S012-S015): **1-1.5 hours**
- AI execution: 30 minutes (build, run tests)
- Human review: 30-45 minutes (review test results, investigate failures)
- Correction cycles: 15 minutes (fix any test failures)

**Phase 5: Code Quality & Standards** (S016-S018): **45-60 minutes**
- AI execution: 20 minutes (run full test suite, review code)
- Human review: 25-40 minutes (validate error handling, check logging)

**Phase 6: Manual API Testing** (S019-S022): **2-3 hours**
- Human-led testing: 120-180 minutes (sandbox API calls, time threshold validation, Polly retry testing)
- Note: Dependent on Asan Pardakht sandbox availability and responsiveness

**Phase 7: Release Preparation** (S023-S025): **30-45 minutes**
- AI execution: 15 minutes (update docs)
- Human review: 15-30 minutes (deployment checklist validation)

### Knowledge Gap Risk Factors

**🟢 Low Risk**: Settlement API integration (well-documented internal task specification)
- Clear API contract with all status codes specified
- Existing BehPardakht pattern provides proven implementation reference
- All infrastructure (Polly, HttpClient) already configured
- Minimal correction cycles expected

**Risk Assessment**:
- **API Documentation**: Excellent (internal specification with examples) → +10% review time
- **Pattern Familiarity**: High (BehPardakht settlement exists) → Minimal additional time
- **Infrastructure Setup**: Complete (Polly, HttpClient ready) → No additional time
- **Testing Complexity**: Moderate (time threshold edge cases) → +15% review time for validation

### Total Estimated Review Time

**Core Development** (Phases 1-5): **6.5-8.5 hours**
**Manual Testing** (Phase 6): **2-3 hours**
**Release Preparation** (Phase 7): **0.5-0.75 hours**

**Total Implementation Time**: **9-12 hours**

**Risk-Adjusted Time** (with 25% buffer for unforeseen issues): **11-15 hours**

> **💡 TIME COMPOSITION**:
> - AI Implementation: ~20% (Claude Code implements code quickly)
> - Human Review: ~45% (reviewing patterns, validating business logic, checking tests)
> - Correction Cycles: ~15% (minor refinements, test fixes)
> - Manual Testing: ~20% (sandbox API testing, time threshold validation)

> **📊 CONFIDENCE LEVEL**: High
> - Clear specification with all requirements defined
> - Proven pattern (BehPardakht) to follow
> - Existing infrastructure ready
> - Limited scope (2 files modified, minimal new code)

## Implementation Structure *(AI guidance)*

### Task Numbering Convention
- **Format**: `S###` with sequential numbering (S001, S002, S003...)
- **Parallel Markers**: `[P]` for tasks that can run concurrently
- **Dependencies**: Clear prerequisite task references
- **File Paths**: Specific target files for each implementation task

### Progress Tracking & Session Continuity
- **This file is the progress tracker** - Check off tasks as `[x]` when complete
- **Sessions are resumable** - New sessions read this file to see what's done
- **Token limits don't matter** - Work can span multiple sessions seamlessly
- **Never rush to completion** - Take the time each step needs for quality
- **TodoWrite is temporary** - Only this file persists across sessions
- **Quality is paramount** - Shortcuts and speed optimizations are forbidden

### Parallel Execution Rules
- **Different files** = `[P]` parallel safe
- **Same file modifications** = Sequential only
- **Independent components** = `[P]` parallel safe
- **Shared resources** = Sequential only
- **Tests with implementation** = Can run `[P]` parallel

### Manual User Action Format
For complex Xcode operations (target creation, scheme setup), use standardized format:
```
═══════════════════════════════════════════════════
║ 🎯 MANUAL XCODE ACTION REQUIRED
═══════════════════════════════════════════════════
║
║ [Step-by-step Xcode UI instructions]
║ [Specific menu paths and actions]
║
║ Reply "Done" when completed to continue.
```

### Quality Integration
*Built into implementation phases, not separate agent tasks*

- **Code Standards**: Follow Context/Guidelines patterns throughout
- **Error Handling**: Apply ErrorKit patterns during service implementation
- **UI Guidelines**: Follow SwiftUI patterns during UI implementation
- **Testing Coverage**: Include test tasks for each implementation phase
- **Platform Compliance**: Consider iOS/macOS requirements in each phase

## Dependency Analysis

### Critical Path
**Longest dependency chain** (cannot be parallelized):

```
S001 (Verify models)
  → S005 (Status code arrays)
    → S006 (SettleErrorHandler)
      → S007 (Settle() implementation)
        → S010 (Failed settlement helper method)
          → S011 (VerifyTransactionQueryHandler integration)
            → S012 (Build solution)
              → S013/S014/S015 (Run tests)
                → S016 (Full test suite)
                  → S019-S022 (Manual testing)
                    → S023-S025 (Release prep)
```

**Estimated critical path time**: 8-10 hours (core development + manual testing)

### Parallel Opportunities
**Tasks that can execute concurrently** (marked with [P]):

**Phase 1 (Setup)**:
- S001 and S002 can run sequentially (quick verification tasks)

**Phase 2 (Provider Implementation)**:
- S003 [P] SettleErrorHandler tests
- S004 [P] Settle() method tests
- *(Tests can be written in parallel while waiting for model verification)*

**Phase 3 (Query Handler)**:
- S008 [P] Date calculation tests
- S009 [P] Integration tests
- *(Test creation can happen in parallel)*

**Phase 4 (Test Execution)**:
- S013 [P] AsanPardakhtProvider tests (after build)
- S014 [P] VerifyTransactionQueryHandler tests (after build)
- *(Independent test suites can run concurrently)*

**Phase 5 (Quality)**:
- S016 [P] Full test suite
- S017 [P] Error handling validation
- *(Code review and test execution can overlap)*

**Phase 7 (Release Prep)**:
- S023 [P] Update feature docs
- S024 [P] Update CLAUDE.md
- *(Documentation updates can happen in parallel)*

**Parallelization Strategy**:
- Test writing: While AI writes tests, human can review previously written code
- Test execution: Multiple test projects can run simultaneously
- Documentation: Multiple documentation files can be updated concurrently

### .NET Technology Dependencies

**Compile-Time Dependencies**:
- SettleTransactionRequest/Response models (S001) → Required by S003, S004, S007
- ServiceType.AsanPardakhtSettle enum (S002) → Required by S007
- Status code arrays (S005) → Required by S006, S007
- SettleErrorHandler (S006) → Required by S007

**Runtime Dependencies**:
- IHttpProvider (existing) → Used by Settle() method
- asanpardakhtClient HttpClient (existing, configured) → Used by IHttpProvider
- Polly policies (existing, configured) → Automatic retry/circuit breaker
- PollyLoggingHandler (existing) → Automatic request/response logging

**Database Dependencies**:
- IPGTransaction entity (existing) → Status property updated
- Transaction entity (existing) → PredictedSettlementDateTime property updated
- AuditableEntityInterceptor (existing) → Automatic ModificationDate updates
- IAggregateRepository<Transaction> (existing) → SaveChanges persistence

**Infrastructure Dependencies** (all pre-existing, no setup needed):
- DependencyInjection.cs → asanpardakhtClient already configured
- PollyPolicyService → Retry/circuit breaker policies ready
- Serilog/Elasticsearch → Logging infrastructure operational
- EF Core WriteDbContext → Database persistence ready

## Completion Verification *(mandatory)*

### Implementation Completeness
- [x] All user scenarios from Spec.md have corresponding implementation tasks?
  - ✅ Scenario 1 (Successful settlement - first call): S007, S011, S019
  - ✅ Scenario 2 (Subsequent settlement - 474 response): S006 (status mapping), S019 (manual test)
  - ✅ Scenario 3 (Unverified transaction - 472): S006 (status mapping), S020 (manual test)
  - ✅ Scenario 4 (Settlement failure): S006 (status codes 471/473/475/478), S011 (failed date calc)
  - ✅ Scenario 5 (Non-Asan Pardakht): S011 (provider type check)

- [x] All architectural components from Tech.md have creation/modification tasks?
  - ✅ AsanPardakhtProvider.Settle(): S005, S006, S007
  - ✅ SettleErrorHandler: S006
  - ✅ VerifyTransactionQueryHandler integration: S011
  - ✅ GetPredictedSettlementDateTimeForFailedSettlement: S010
  - ✅ Communication models verification: S001

- [x] Error handling and edge cases covered in task breakdown?
  - ✅ Status code mapping (200/474/476 → 9, others → 10): S006
  - ✅ Time threshold boundaries (23:45, 20:40): S008, S021 (manual tests)
  - ✅ Provider type filtering: S011
  - ✅ Polly retry/circuit breaker: S017, S022 (manual test)
  - ✅ Missing ProviderData: S017 (validation)

- [x] Performance requirements addressed in implementation plan?
  - ✅ Polly policies validated: S017, S022
  - ✅ No additional infrastructure needed (reuse existing)
  - ✅ Settlement within same DB transaction: S011

- [x] .NET backend-specific requirements integrated throughout phases?
  - ✅ Clean Architecture layer separation maintained
  - ✅ CQRS pattern preserved (settlement in query handler per BehPardakht pattern)
  - ✅ TDD approach with unit/integration tests
  - ✅ Polly resilience policies leveraged
  - ✅ Entity Framework auditing (ModificationDate automatic)

### Quality Standards
- [x] Each task specifies exact file paths and dependencies?
  - ✅ All S001-S025 tasks have explicit file paths
  - ✅ Dependencies clearly stated (e.g., "S007 depends on S006, S001, S002")
  - ✅ Build/test commands specify exact projects and filters

- [x] Parallel markers `[P]` applied correctly for independent tasks?
  - ✅ Test writing tasks marked [P] (S003, S004, S008, S009)
  - ✅ Test execution tasks marked [P] (S013, S014, S016, S017)
  - ✅ Documentation tasks marked [P] (S023, S024)
  - ✅ Sequential tasks (S005 → S006 → S007) not marked parallel

- [x] Test tasks included for all major implementation components?
  - ✅ SettleErrorHandler: S003
  - ✅ AsanPardakhtProvider.Settle(): S004
  - ✅ GetPredictedSettlementDateTimeForFailedSettlement: S008
  - ✅ End-to-end settlement flow: S009
  - ✅ Manual API testing: S019-S022

- [x] Code standards and guidelines referenced throughout plan?
  - ✅ Follow existing AsanPardakhtProvider patterns (S005, S006, S007)
  - ✅ Mirror BehPardakht settlement pattern (S011)
  - ✅ Use DateTime.Now per codebase convention (S010)
  - ✅ Clean Architecture compliance validation (S017)

- [x] No implementation details that should be in tech plan?
  - ✅ Tech.md contains architectural decisions and code snippets
  - ✅ Steps.md contains task breakdown and execution order
  - ✅ Clear separation maintained

### Release Readiness
- [x] Privacy and compliance considerations addressed?
  - ✅ No new PII collected (uses existing transaction data)
  - ✅ API credentials handled via existing ProviderData pattern
  - ✅ Logging configured via existing PollyLoggingHandler

- [x] Documentation and release preparation tasks included?
  - ✅ Feature documentation updates: S023
  - ✅ CLAUDE.md updates: S024
  - ✅ Deployment checklist: S025
  - ✅ Pipeline deployment guidance provided

- [x] Feature branch ready for systematic development execution?
  - ✅ Branch created: feature/001-asan-pardakht-settlement
  - ✅ All prerequisite files complete (Spec.md, Research.md, Tech.md)
  - ✅ Implementation plan validated and comprehensive

- [x] All milestones defined with appropriate commit guidance?
  - ✅ 6 milestones with commit message templates
  - ✅ All milestones specify "Use Task tool with commit-changes agent"
  - ✅ Progressive commits for: Prerequisites, Provider, Query Handler, Testing, Quality, Release

---

**Next Phase**: After implementation steps are completed, proceed to `/ctxk:impl:start-working` to begin systematic development execution.