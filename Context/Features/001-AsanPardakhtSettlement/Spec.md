# Feature Specification: Asan Pardakht Settlement Service Integration

**Feature Branch**: `feature/001-asan-pardakht-settlement`
**Created**: 2025-10-17
**Status**: Draft
**Input**:
"""
من به عنوان برنامه کاربردی میخواهم تا در صورت تایید یک درخواست پرداخت، اگر متناظر این درخواست پرداخت یک تراکنش از نوع IPG با سرویس دهنده آسان پرداخت وجود داشت، سیستم نسبت به فراخوانی سرویس تسویه تراکنش اقدام کند

نیاز است تا پس از فراخوانی سرویس Verify (از سرویس های آپ) در صورتیکه عملیات تایید موفقیت آمیز صورت گرفته باشد، اگر نوع سرویس دهنده مربوط به درگاه مربوط به درخواست پرداخت : "آسان پرداخت" باشد، در گام بعدی نسبت به فراخوانی سرویس تسویه تراکنش اقدام شود

متد settle بنویس AsanPardakhtProvider این سرویس رو در پروایدر

بعدش برو به VerifyTransactionQueryHandler اینو براش بنویس
نیاز است تا پس از فراخوانی سرویس Settlement (آسان پرداخت) در صورتیکه هیچ خطا یا مشکلی بروز نکند، متناسب با خروجی سرویس مذکور نسبت به بروز رسانی موجودیت های IPG_Transaction و Transaction اقدام شود
"""

## User Scenarios & Testing *(mandatory)*

### Primary User Story
As a **payment gateway system**, I want to automatically submit verified Asan Pardakht IPG transactions for settlement so that merchants receive their funds within the expected settlement timeframe and the system maintains accurate transaction status tracking.

**Platform Context**:
- **Payment Flow Integration**: Settlement request occurs automatically after successful payment verification for Asan Pardakht provider
- **Transaction Lifecycle**: Settlement is a critical post-verification step that initiates the funds transfer process
- **Data Consistency**: System must maintain accurate status tracking and predicted settlement dates for reporting and reconciliation

### Acceptance Scenarios

#### Scenario 1: Successful Settlement Request (First Call)
1. **Given** a payment request has been successfully verified, **And** the IPG provider is Asan Pardakht, **And** the settlement service has not been called yet, **When** the system calls the Asan Pardakht Settlement API, **Then** the API returns HTTP 200 status code, **And** the system updates IPG_Transaction status to "9" (settlement request successful), **And** the system calculates and stores the predicted settlement date/time based on current time
   - **Happy Path**: Settlement request accepted on first call with 200 response
   - **Business Rule**: If current time < 23:45, predicted settlement = (current date + 1 day) at 07:00; otherwise (current date + 2 days) at 07:00

#### Scenario 2: Subsequent Settlement Request (Already Pending)
2. **Given** a settlement request has already been submitted successfully, **When** the system calls the Settlement API again for the same transaction, **Then** the API returns HTTP 474 status code with error code 1026 "Transaction Is Pending for Reconcilation", **And** the system updates IPG_Transaction status to "9" (settlement request successful), **And** the system updates the predicted settlement date/time
   - **Expected Behavior**: Status codes 474 and 476 are treated as successful settlement states

#### Scenario 3: Settlement Request for Unverified Transaction
3. **Given** a payment transaction exists, **And** the Verify service has not been called for this transaction, **When** the system attempts to call the Settlement API, **Then** the API returns HTTP 472 status code with error code 1029 "Unverified Transaction", **And** the system updates IPG_Transaction status to "10" (settlement request failed), **And** the system calculates a later predicted settlement date (current time < 20:40: current date + 1 day at 07:00, otherwise current date + 2 days at 07:00)
   - **Error Path**: Settlement cannot proceed without prior verification

#### Scenario 4: Settlement Request Failure
4. **Given** a verified payment request with Asan Pardakht provider, **When** the Settlement API returns error status codes (471, 473, 475, 478), **Then** the system updates IPG_Transaction status to "10" (settlement request failed), **And** the system calculates predicted settlement date based on failed settlement rules (current time threshold: 20:40 instead of 23:45)
   - **Error Path**: Various service failures result in failed settlement status

#### Scenario 5: Non-Asan Pardakht Provider
5. **Given** a successfully verified payment request, **And** the IPG provider is NOT Asan Pardakht, **When** the verification process completes, **Then** the system does NOT call the Settlement API, **And** no settlement-specific status updates occur
   - **Boundary Condition**: Settlement service is Asan Pardakht-specific only

### Edge Cases
- **Time Boundary Conditions**:
  - Settlement request at exactly 23:45:00 (successful status) - should predicted date be +1 or +2 days?
  - Settlement request at exactly 20:40:00 (failed status) - should predicted date be +1 or +2 days?
- **API Timeout/Network Errors**: Settlement API call fails due to timeout or network connectivity - system should handle gracefully with retry logic (Polly policies)
- **Multiple Concurrent Requests**: Same transaction settlement called multiple times simultaneously - system should handle idempotently
- **Missing Provider Configuration**: Company_IPG.provider_data missing required fields (merchantConfigurationId, username, password) - system should validate before API call
- **Invalid Provider Tracker ID**: IPG_Transaction.provider_tracker_id is null or invalid - system should prevent Settlement API call
- **Settlement After Retry Exhaustion**: After multiple Polly retries, settlement still fails - system should mark as failed (status "10") and log appropriately
- **Date/Time Calculation Edge Cases**: Settlement request just before midnight - ensure date calculations use consistent timezone and handle daylight saving time transitions

## Requirements *(mandatory)*

### Functional Requirements

#### Core Settlement Flow
- **FR-001**: System MUST automatically invoke the Settlement API after successful payment verification ONLY when the IPG provider is Asan Pardakht
  - **Success Criteria**: Settlement API call triggered only for verified transactions with Asan Pardakht provider; no Settlement call for other providers

- **FR-002**: System MUST send Settlement API requests to `https://ipgrest.asanpardakht.ir/v1/Settlement` with:
  - Request body containing: `merchantConfigurationId` (from Company_IPG.provider_data.Merchant_Configuration_Id) and `payGateTranId` (from IPG_Transaction.provider_tracker_id)
  - **Success Criteria**: API receives correctly formatted JSON request with valid merchant and transaction identifiers

- **FR-003**: System MUST include authentication credentials in Settlement API request headers:
  - `usr` header from Company_IPG.provider_data.User_Name
  - `pwd` header from Company_IPG.provider_data.Password
  - **Success Criteria**: API accepts authentication and processes request

#### Response Handling & Status Mapping
- **FR-004**: System MUST interpret Settlement API responses and map to transaction status:
  - HTTP 200, 474, or 476 → IPG_Transaction.status = "9" (settlement request successful)
  - HTTP 471, 472, 473, 475, or 478 → IPG_Transaction.status = "10" (settlement request failed)
  - **Success Criteria**: IPG_Transaction status accurately reflects settlement outcome per API response

- **FR-005**: System MUST update IPG_Transaction.modification_date_time with current timestamp when settlement status is updated
  - **Success Criteria**: modification_date_time reflects when settlement processing occurred

#### Predicted Settlement Date Calculation
- **FR-006**: System MUST calculate Transaction.predicted_settlement_date_time based on settlement status and current time:
  - **For successful settlement (status = "9")**:
    - If current time < 23:45 → predicted date = (current date + 1 day) at 07:00
    - If current time >= 23:45 → predicted date = (current date + 2 days) at 07:00
  - **For failed settlement (status = "10")**:
    - If current time < 20:40 → predicted date = (current date + 1 day) at 07:00
    - If current time >= 20:40 → predicted date = (current date + 2 days) at 07:00
  - **Success Criteria**: Predicted settlement dates calculated correctly based on status and time thresholds

- **FR-007**: System MUST update Transaction.modification_date_time with current timestamp when predicted settlement date is calculated
  - **Success Criteria**: modification_date_time reflects when settlement prediction was updated

#### Data Validation & Error Handling
- **FR-008**: System MUST validate required data exists before calling Settlement API:
  - Company_IPG.provider_data contains merchantConfigurationId, username, and password
  - IPG_Transaction.provider_tracker_id is not null
  - **Success Criteria**: Settlement API call only proceeds when all required data is available; appropriate error logged if data missing

- **FR-009**: System MUST handle Settlement API errors gracefully with retry logic (using existing Polly policies):
  - Retry on transient errors (5xx, timeouts, network errors)
  - Log all retry attempts and final outcomes
  - **Success Criteria**: Transient failures automatically retried; persistent failures marked as status "10" with detailed logging

#### Timezone & Date Handling
- **FR-010**: System MUST use server local time (DateTime.Now) for all settlement date/time calculations and time comparisons (23:45, 20:40 thresholds)
  - **Rationale**: Existing codebase uses DateTime.Now in VerifyTransactionQueryHandler and other IPG transaction handlers
  - **Success Criteria**: All settlement-related timestamps and comparisons use DateTime.Now consistently with existing IPG transaction code

## Scope Boundaries *(mandatory)*

- **IN SCOPE**:
  - Asan Pardakht Settlement API integration after successful payment verification
  - Provider-specific conditional settlement call (only for Asan Pardakht)
  - Settlement request construction with merchant credentials and transaction ID
  - Settlement API response handling with status code mapping (200/474/476 → success, 471/472/473/475/478 → failed)
  - IPG_Transaction entity updates (status, modification_date_time)
  - Transaction entity updates (predicted_settlement_date_time, modification_date_time)
  - Predicted settlement date calculation based on business rules (different time thresholds for successful vs. failed settlements)
  - Data validation before Settlement API call
  - Resilience and retry handling using existing Polly policies
  - Comprehensive logging of settlement requests, responses, and errors

- **OUT OF SCOPE**:
  - Settlement functionality for other IPG providers (only Asan Pardakht in this iteration)
  - Manual settlement triggering or administrative override mechanisms
  - Settlement reconciliation or verification workflows
  - Webhook or callback handling from Asan Pardakht for settlement status updates
  - Settlement reporting or dashboard features
  - Retry logic configuration changes (use existing Polly configuration)
  - Actual funds transfer processing (handled by Asan Pardakht, not our system)
  - Settlement date range or calendar business day calculations (use simple +1 or +2 day logic as specified)
  - Multi-currency settlement considerations

