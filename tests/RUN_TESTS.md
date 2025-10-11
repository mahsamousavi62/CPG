# Quick Test Execution Guide

## Prerequisites

```bash
cd /home/mehrdad/repo/CPG
```

## Run All Logging Tests

```bash
# Fastest: Run all logging tests
dotnet test tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj \
  --filter "FullyQualifiedName~Logging" \
  --configuration Release

# With detailed output
dotnet test tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj \
  --filter "FullyQualifiedName~Logging" \
  --logger "console;verbosity=detailed"
```

## Run Individual Test Files

```bash
# Model Tests
dotnet test --filter "FullyQualifiedName~CallLogModelTests"
dotnet test --filter "FullyQualifiedName~RequestResponseLogModelTests"

# Service Tests
dotnet test --filter "FullyQualifiedName~LogServiceTests"

# Middleware Tests
dotnet test --filter "FullyQualifiedName~LoggingMiddlewareTests"

# Policy/Resilience Tests
dotnet test --filter "FullyQualifiedName~PollyLoggingHandlerTests"
dotnet test --filter "FullyQualifiedName~SoapLoggingWrapperTests"

# Validation Tests
dotnet test --filter "FullyQualifiedName~AuditLogValidationTests"
```

## Test File Locations

```
tests/CPG.Domain.Tests.Unit/
├── SharedKernel/
│   └── Logging/
│       ├── CallLogModelTests.cs              (10 tests)
│       └── RequestResponseLogModelTests.cs    (15 tests)
├── Infrastructure/
│   ├── Logging/
│   │   ├── LogServiceTests.cs                (15 tests)
│   │   ├── LoggingMiddlewareTests.cs         (15 tests)
│   │   └── AuditLogValidationTests.cs        (9 tests)
│   └── Policies/
│       ├── PollyLoggingHandlerTests.cs       (13 tests)
│       └── SoapLoggingWrapperTests.cs        (14 tests)
```

**Total: 91 tests**

## Expected Output

When all tests pass, you should see:
```
Passed!  - Failed:     0, Passed:    91, Skipped:     0, Total:    91
```

## Troubleshooting

### If tests fail to build:
1. Restore packages: `dotnet restore --configfile nuget.config`
2. Clean: `dotnet clean`
3. Rebuild: `dotnet build tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj`

### If specific tests fail:
1. Check test output for error messages
2. Review the specific test file
3. Verify implementation matches test expectations

## Test Documentation

- **Scenarios**: `AUDIT_LOG_TEST_SCENARIOS.md`
- **Summary**: `TESTING_VALIDATION_SUMMARY.md`
- **Quick Guide**: `RUN_TESTS.md` (this file)

## Questions?

Refer to `/home/mehrdad/repo/CPG/CLAUDE.md` for project conventions and architecture.
