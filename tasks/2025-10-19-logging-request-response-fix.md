# Logging Request/Response Bodies During Timeouts and Exceptions

## Overview
Fix the logging infrastructure across all payment gateway providers to ensure complete request and response bodies are captured during timeout scenarios and exceptions. Currently, request bodies are being lost or partially reconstructed during error handling, making debugging and troubleshooting extremely difficult.

## Architecture Alignment
This task maintains the existing logging architecture using CallLogModel factory methods while improving data capture reliability. The changes align with the Clean Architecture pattern by enhancing infrastructure layer logging without modifying domain or application layers. All modifications are confined to the Infrastructure layer providers.

## Root Cause Analysis

### HttpProvider.cs Issue
The `CreateErrorCallLogModel` method (line 631-652) loses all request data:
- `requestBody: null` - Complete loss of request data
- `responseBody: exception.Message` - Only error message, no actual response body
- This affects ALL HTTP methods: PostAsync, PostAsync3, PostAsync4, GetAsync, PutAsync, PatchAsync

### SOAP Provider Issues
Both BehPardakhtProvider.cs and PecProvider.cs manually reconstruct partial request objects in catch blocks:
- Only selected fields are logged (amount, IDs, etc.)
- Full request structure is lost
- Makes debugging incomplete payments impossible

### CharisPayProvider.cs Issue
In GetAccountNumber method catch blocks (lines 109, 127):
- Manually serializes only `{ iban }` instead of full request
- The `json` variable containing the full request already exists at line 50 but is not reused

## Tasks

### 1. Fix HttpProvider CreateErrorCallLogModel Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Medium

- [x] Modify CreateErrorCallLogModel signature to accept optional requestBody parameter (string)
- [x] Modify CreateErrorCallLogModel signature to accept optional responseBody parameter (string)
- [x] Update CreateErrorCallLogModel to use passed requestBody instead of null
- [x] Update CreateErrorCallLogModel to use passed responseBody if available, fallback to exception.Message
- [x] Apply PII masking to requestBody parameter using Regex.Replace(requestBody, Constants.Pattern, Constants.Replaceformat)
- [x] Apply PII masking to responseBody parameter using Regex.Replace(responseBody, Constants.Pattern, Constants.Replaceformat)

### 2. Fix HttpProvider PostAsync Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] Serialize request.Body to JSON before the try block (reuse existing approach from PostAsync3/PostAsync4)
- [x] Apply PII masking to serialized request body
- [x] In catch block, pass serialized request body to CreateErrorCallLogModel
- [x] Handle case where request might be null (use request?.Body null-conditional)

### 3. Fix HttpProvider PostAsync3 Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] Move json variable declaration before try block (currently at line 135)
- [x] Apply PII masking to json variable after serialization
- [x] In catch block, pass json variable to CreateErrorCallLogModel as requestBody parameter
- [x] Ensure json variable is accessible in catch block scope

### 4. Fix HttpProvider PostAsync4 Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] Move json variable declaration before try block (currently at line 206)
- [x] Apply PII masking to json variable after serialization
- [x] In catch block, pass json variable to CreateErrorCallLogModel as requestBody parameter
- [x] Ensure json variable is accessible in catch block scope

### 5. Fix HttpProvider PutAsync Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] Serialize request.Body to JSON before the try block
- [x] Apply PII masking to serialized request body
- [x] In catch block, pass serialized request body to CreateErrorCallLogModel
- [x] Handle case where request might be null

### 6. Fix HttpProvider PatchAsync Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] Serialize request.Body to JSON before the try block
- [x] Apply PII masking to serialized request body
- [x] In catch block, pass serialized request body to CreateErrorCallLogModel
- [x] Handle case where request might be null

### 7. Fix HttpProvider GetAsync Methods ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Estimated Complexity**: Low

- [x] For GetAsync<TBaseRequest, TResponse, TError, TBody>, serialize request.Body before try block
- [x] For GetAsync<TRequest, TResponse, TBody> overload, serialize request object before try block
- [x] Apply PII masking to all serialized request bodies
- [x] Update both catch blocks to pass serialized request to CreateErrorCallLogModel
- [x] Handle null request scenarios properly

### 8. Fix BehPardakhtProvider GetPaymentTokenAsync Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Medium

- [x] Serialize the full payRequest object to JSON before the try block (after line 81)
- [x] Apply PII masking using Regex.Replace with Constants.Pattern
- [x] Store serialized request in a variable (e.g., requestBodyJson)
- [x] Update catch block (line 114) to use requestBodyJson instead of manually constructed object
- [x] Ensure serialized request is accessible in both catch block and nested Retry method

### 9. Fix BehPardakhtProvider Verify Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Medium

- [x] Serialize the full verifyRequest object to JSON before the try block
- [x] Apply PII masking to serialized request
- [x] Store in variable accessible to catch block
- [x] Update catch block (line 82) to use full serialized request instead of partial object
- [x] Ensure serialized request is accessible in nested Retry method

### 10. Fix BehPardakhtProvider Settle Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Medium

- [x] Serialize the full settleRequest object to JSON before the try block
- [x] Apply PII masking to serialized request
- [x] Store in variable accessible to catch block
- [x] Update catch block (line 149) to use full serialized request instead of partial object
- [x] Ensure serialized request is accessible in nested Retry method

### 11. Fix PecProvider GetPaymentTokenAsync Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Low

- [x] Serialize the full clientSaleRequestData object to JSON before the try block (after line 53)
- [x] Apply PII masking using Regex.Replace with Constants.Pattern
- [x] Store serialized request in a variable
- [x] Update catch block (line 79) to use full serialized request instead of partial object

### 12. Fix PecProvider Verify Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Low

- [x] Serialize the full request object (ClientConfirmRequestData) to JSON before the try block (after line 38)
- [x] Apply PII masking to serialized request
- [x] Store in variable accessible to catch block
- [x] Update catch block (line 55) to use full serialized request instead of partial object

### 13. Fix CharisPayProvider GetAccountNumber Method ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Estimated Complexity**: Low

- [x] Reuse existing json variable from line 50 in both catch blocks
- [x] Apply PII masking to json variable after it's created (line 50)
- [x] Update HttpRequestException catch block (line 109) to use json instead of JsonConvert.SerializeObject(new { iban })
- [x] Update Exception catch block (line 127) to use json instead of JsonConvert.SerializeObject(new { iban })
- [x] Ensure json variable scope is accessible to both catch blocks

## Execution Order Recommendations

### Phase 1 - HttpProvider Foundation (Tasks 1-7)
**Sequential execution required**
1. Complete Task 1 first (CreateErrorCallLogModel signature change)
2. Then execute Tasks 2-7 sequentially to update all HTTP method catch blocks
3. This phase MUST be completed before deploying, as signature change affects all methods

**Rationale**: The signature change in Task 1 is a breaking change that all HTTP methods depend on.

### Phase 2 - SOAP Providers (Tasks 8-12)
**Can be parallelized**
- Tasks 8-10 (BehPardakht) can be done by one developer
- Tasks 11-12 (Pec) can be done by another developer in parallel
- No interdependencies between these tasks

**Rationale**: SOAP providers are independent of HttpProvider changes and independent of each other.

### Phase 3 - CharisPayProvider (Task 13)
**Can be done anytime**
- Independent of all other tasks
- Can be done in parallel with Phase 1 or Phase 2

**Rationale**: CharisPayProvider uses HttpClient directly, not HttpProvider methods.

### Deployment Strategy
- **CRITICAL**: All tasks in Phase 1 (Tasks 1-7) must be deployed together atomically
- Phase 2 and 3 can be deployed independently or together with Phase 1
- No partial deployment of Phase 1 is possible due to method signature change

## Potential Risks & Considerations

### Technical Risks
1. **Memory Impact**: Serializing request bodies before try blocks adds memory overhead
   - **Mitigation**: Requests are already being serialized in success paths, so net impact is minimal
   - Only adds one string variable per method call

2. **Performance**: Additional serialization on error paths
   - **Mitigation**: Error paths are exceptional cases, performance hit is acceptable
   - Success path performance is unchanged

3. **Variable Scoping**: Serialized request variables must be accessible in catch blocks
   - **Mitigation**: Declare variables before try blocks at method scope
   - Ensure nested retry methods can access the variables if needed

4. **PII Masking Consistency**: Must apply Constants.Pattern masking consistently
   - **Mitigation**: Use same pattern as existing AddServiceCallLog methods
   - Apply masking immediately after serialization

### Architectural Considerations
1. **Breaking Change in HttpProvider**: CreateErrorCallLogModel signature change
   - **Impact**: Only affects internal HttpProvider methods, not public API
   - **Mitigation**: All callers are in same file, can be updated atomically

2. **SOAP Provider Retry Logic**: Serialized request must be accessible in retry methods
   - **Impact**: Retry methods are nested local functions with closure access
   - **Mitigation**: Variables declared at method scope are automatically captured

3. **Backward Compatibility**: Log structure changes
   - **Impact**: Elasticsearch queries expecting null requestBody may need updates
   - **Mitigation**: Adding data is backward compatible, consumers can ignore new fields

### Implementation Guidelines
1. **Serialization Consistency**:
   - HttpProvider: Use System.Text.Json.JsonSerializer.Serialize (matches existing code)
   - SOAP Providers: Use JsonConvert.SerializeObject (matches existing code)
   - CharisPayProvider: Already uses JsonConvert.SerializeObject

2. **PII Masking Pattern**:
   ```csharp
   string requestBodyJson = JsonConvert.SerializeObject(request);
   if (!string.IsNullOrEmpty(requestBodyJson))
   {
       requestBodyJson = Regex.Replace(requestBodyJson, Constants.Pattern, Constants.Replaceformat);
   }
   ```

3. **Null Safety**:
   - Always check for null requests before serialization
   - Use null-conditional operators where appropriate
   - Pass null to CreateErrorCallLogModel if request is null

4. **Exception Information Preservation**:
   - Still pass full exception object to CallLogModel.CreateError
   - responseBody parameter should contain actual response if available
   - Fallback to exception.Message only when no response body exists

---

## 📊 Execution Summary

**Status:** ✅ **COMPLETED**
**Date:** 2025-10-19
**Agent:** parallel-task-executor

### Statistics:
- **Total Tasks:** 13
- **✅ Completed:** 13
- **⏭️ Skipped:** 0
- **❌ Failed:** 0
- **Files Modified:** 4

### Tasks Executed:
1. ✅ Fix HttpProvider CreateErrorCallLogModel Method - Sequential
2. ✅ Fix HttpProvider PostAsync Method - Sequential
3. ✅ Fix HttpProvider PostAsync3 Method - Sequential
4. ✅ Fix HttpProvider PostAsync4 Method - Sequential
5. ✅ Fix HttpProvider PutAsync Method - Sequential
6. ✅ Fix HttpProvider PatchAsync Method - Sequential
7. ✅ Fix HttpProvider GetAsync Methods - Sequential
8. ✅ Fix BehPardakhtProvider GetPaymentTokenAsync Method - Sequential
9. ✅ Fix BehPardakhtProvider Verify Method - Sequential
10. ✅ Fix BehPardakhtProvider Settle Method - Sequential
11. ✅ Fix PecProvider GetPaymentTokenAsync Method - Sequential
12. ✅ Fix PecProvider Verify Method - Sequential
13. ✅ Fix CharisPayProvider GetAccountNumber Method - Sequential

### Code Changes:
```
4 files modified:
- /home/mehrdad/repo/CPG/src/CPG.Infrastructure/Providers/HttpProvider.cs
- /home/mehrdad/repo/CPG/src/CPG.Infrastructure/Providers/Ipg/BehPardakhtProvider.cs
- /home/mehrdad/repo/CPG/src/CPG.Infrastructure/Providers/Ipg/PecProvider.cs
- /home/mehrdad/repo/CPG/src/CPG.Infrastructure/Providers/Charispay/CharisPayProvider.cs
```

### Key Improvements:
- ✅ Full request bodies now logged during timeout/exception scenarios
- ✅ PII masking applied consistently using Constants.Pattern
- ✅ All SOAP providers serialize complete request objects
- ✅ All REST methods preserve full request data in error paths
- ✅ Backward compatible - log structure enhanced, not changed
