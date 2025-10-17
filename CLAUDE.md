# CLAUDE.md

@Context.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CPG is a payment gateway REST API built with .NET 8 using Clean Architecture (Hexagonal Architecture + DDD + CQRS). The system handles payment requests, IPG (Internet Payment Gateway) integrations, direct debit, and various payment methods including CharismaCard and payment receipts.

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

### Layer Dependencies (Inner to Outer)
1. **CPG.Domain** - Core business logic and domain models (no dependencies)
   - Contains Aggregate Models in AggregateModels/ organized by business domain (Application, Bank, Company, CompanyDeposit, CompanyIPG, DirectDebitGrant, IPGType, PaymentRequest, Provider, Transaction, User, AuditLog)
   - Uses DDD patterns: Entities, Value Objects, Domain Events, Aggregate Roots
   - Shared Kernel (SharedKernel/) contains base classes and common types: Entity, AuditableEntity, IAggregateRoot, ApplicationSettingsAggregate

2. **CPG.Application** - Application business rules and use cases
   - Organized by feature folders under UseCases/ (Applications, Auth, Banks, CharismaCard, CharisPayServices, Companies, CompanyDeposits, CompanyIPGs, DirectDebit, Files, Ipg, IPGResult, IPGType, NeoBankServices, PaymentReceipt, PaymentRequests, Providers, Users, etc.)
   - Implements CQRS pattern with Commands and Queries
   - Uses MediatR for request handling with Pipeline Behaviours in Shared/Behaviours/ (AuthorizationBehaviour, UnhandledExceptionBehaviour, PerformanceBehaviour)
   - Uses FluentValidation for request validation
   - Validation handlers organized using Chain of Responsibility pattern for complex validations
   - Depends only on Domain layer

3. **CPG.Infrastructure** - External service integrations
   - Contains implementations for: Kafka, RabbitMQ, Masstransit, Minio, Cache, Logging, Time, File handling
   - Provider integrations in Providers/ for external payment services: CharismaCard, Charispay, DirectDebit, Idp, Ipg, NeoBank
   - Polly resilience policies in Policies/: PollyPolicyService.cs, PolicyConfig.cs
   - Depends on Application and Domain

4. **CPG.Infrastructure.Persistence** - Data access layer
   - Uses Entity Framework Core 8.0 with SQL Server
   - CQRS read/write separation: ReadDbContext (queries) and WriteDbContext (commands)
   - GraphQL support via HotChocolate for queries
   - Repository pattern with Specification pattern (Ardalis.Specification)
   - Interceptors in Interceptors/: AuditableEntityInterceptor, DispatchDomainEventsInterceptor, DatabaseLoggingInterceptor
   - Redis caching support
   - Depends on Application and Domain

5. **CPG.Infrastructure.Authorization** - Authentication and authorization
   - JWT token handling
   - Authentication middleware
   - Current user context management
   - External services middleware
   - Depends on Application and Domain

6. **CPG.API** - Presentation layer
   - ASP.NET Core 8.0 Web API
   - Swagger/OpenAPI documentation with bearer token support
   - API versioning
   - Serilog logging with Elasticsearch, Console, and File sinks
   - Localization support
   - Mapster for object mapping
   - Depends on all infrastructure layers

### Key Architectural Patterns

- **CQRS**: Commands modify state, Queries return data (separate DbContexts)
- **Mediator Pattern**: All requests flow through MediatR handlers
- **Repository Pattern**: Data access abstracted through repositories
- **Specification Pattern**: Complex query logic encapsulated in reusable specifications
- **Chain of Responsibility**: Validation handlers for complex multi-step validation (e.g., CreatePaymentRequest)
- **Strategy Pattern**: Payment method validation strategies (e.g., AvailableIpgStrategy)
- **Domain Events**: Domain changes trigger events handled by event handlers

### Domain Model Organization

Aggregate roots in src/CPG.Domain/AggregateModels/ are organized by business domain:
- **ApplicationAggregate**: OAuth applications and API clients
- **AuditLogAggregate**: Audit logging for system events
- **BankAggregate**: Bank information and direct debit settings
- **CompanyAggregate**: Merchant companies and their payment methods
- **CompanyDepositAggregate**: Company bank accounts/deposits (IBANs)
- **CompanyIPGAggregate**: Company-specific IPG configurations
- **DirectDebitGrantAggregate**: Direct debit authorization and plans
- **IPGTypeAggregate**: Types of payment gateways
- **PaymentRequestAggregate**: Payment requests and their methods (core aggregate)
- **ProviderAggregate**: External payment providers
- **TransactionAggregate**: Payment transactions (IPG, DirectDebit, CharismaCard, PaymentReceipt)
- **UserAggregate**: System users with roles

Shared kernel in src/CPG.Domain/SharedKernel/ contains:
- Base classes and interfaces
- ApplicationSettingsAggregate for configuration
- Common types and enums

## Common Commands

### Build and Test
```bash
# Clean solution
dotnet clean CPG.sln

# Restore dependencies
dotnet restore CPG.sln --configfile nuget.config

# Build solution
dotnet build CPG.sln --configuration Release

# Run all tests
dotnet test CPG.sln --configuration Release

# Run specific test project
dotnet test tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj --configuration Release

# Run unit tests only
dotnet test CPG.sln --filter FullyQualifiedName~Tests.Unit --configuration Release

# Run architecture tests
dotnet test tests/CPG.Architecture.Tests/CPG.Architecture.Tests.csproj

# List tests in a specific project
dotnet test tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj --list-tests

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~YourTestName"
```

### Database Migrations
```bash
# Set default project to CPG.Infrastructure.Persistence in Package Manager Console

# Add new migration
Add-Migration <migration_name> -Context WriteDbContext -o Migrations

# Update database
Update-Database -Context WriteDbContext
```

### Docker
```bash
# Build and run with Docker Compose
docker-compose up --build

# Run in detached mode
docker-compose up -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```

The API runs on port 5000 (HTTPS) and connects to SQL Server 2019 on port 1433.

### Development
```bash
# Run API locally
dotnet run --project src/CPG.API/CPG.API.csproj

# Watch mode for development
dotnet watch --project src/CPG.API/CPG.API.csproj
```

## CI/CD Pipeline

The project uses Azure DevOps pipelines (azure-pipeline.yaml) with the following stages:
1. Clean and Restore
2. Build (Release configuration)
3. Run Unit Tests
4. Run Integration Tests
5. Publish artifacts
6. Docker build and push (branch-specific):
   - `develop` → cpg/dev/cpg-api-backend
   - `stage` → cpg/stage/cpg-api-backend
   - `master` → cpg/prod/cpg-api-backend

## Important Conventions

### Use Case Organization
- Each use case is in a feature folder under `src/CPG.Application/UseCases/<Feature>/`
- Commands in `Commands/<CommandName>/` containing: Command, CommandHandler, CommandValidator
- Queries in `Queries/<QueryName>/` containing: Query, QueryHandler
- Event handlers in `EventHandlers/` (if applicable)

### Validation Strategy
- FluentValidation for basic request validation
- For complex multi-step validation (e.g., CreatePaymentRequest), use Chain of Responsibility pattern with IValidationHandler implementations

### Domain Events
- Domain events are raised in aggregate roots
- Handled by MediatR event handlers in Application layer
- Dispatched automatically via DispatchDomainEventsInterceptor during SaveChanges

### Resource Files
- Localization resources are in Resource.resx files within each aggregate
- Application resources in `CPG.Application/Shared/Resource/`
- Resource files are compiled as embedded resources

### API Response
- Use Mapster for DTO mapping
- Return appropriate HTTP status codes
- Leverage global error handling middleware

### Testing
- Unit tests in `tests/CPG.Domain.Tests.Unit/`
- Base test utilities in `tests/CPG.Tests.Base/`
- Architecture tests in `tests/CPG.Architecture.Tests/` using NetArchTest.Rules

## Technology Stack

- **.NET 8.0**
- **Entity Framework Core 8.0** (SQL Server)
- **MediatR 12.x** (CQRS)
- **FluentValidation 11.x**
- **HotChocolate 13.x** (GraphQL)
- **Mapster 7.x** (Object mapping)
- **Serilog** (Logging to Elasticsearch, Console, File)
- **Polly 8.x** (Resilience and transient-fault-handling)
- **Ardalis.Specification** (Repository pattern)
- **Ardalis.GuardClauses** (Guard clauses)
- **Redis** (Caching)
- **RabbitMQ/Kafka/MassTransit** (Messaging)
- **Minio** (File storage)
- **xUnit** (Testing framework)
- **NetArchTest.Rules** (Architecture testing)

## Resilience and Retry Policies

The project uses **Polly** for implementing resilience patterns including retry, circuit breaker, and timeout policies.

### Polly Configuration

All resilience policies are centrally managed through `IPollyPolicyService` in `CPG.Infrastructure.Policies`:

**Configuration Location**: `appsettings.json` → `Infrastructure:Polly`

```json
{
  "Infrastructure": {
    "Polly": {
      "Retry": {
        "MaxRetryAttempts": 3,
        "BaseDelaySeconds": 2,
        "UseJitter": true
      },
      "CircuitBreaker": {
        "Enabled": true,
        "FailureThreshold": 5,
        "SamplingDurationSeconds": 30,
        "MinimumThroughput": 7,
        "DurationOfBreakSeconds": 60
      },
      "Timeout": {
        "TimeoutSeconds": 30,
        "SoapTimeoutSeconds": 45
      }
    }
  }
}
```

### HTTP Clients with Polly

**All HttpClients** (both named and default) are configured with Polly policies:

1. **Named HttpClients**: `charisPayClient`, `idpClient`, `neoBankClient`, `asanpardakhtClient`, `charismaCardClient`
2. **Default HttpClient**: Used by `HttpProvider` via `CreateClient()` without name

All clients have:
- Automatic retry with exponential backoff and jitter
- Circuit breaker to prevent cascading failures
- Timeout protection
- Complete request/response logging on timeout via `PollyLoggingHandler`

### SOAP Services with Polly

SOAP service providers use `IPollyPolicyService.ExecuteWithPolicyAsync()`:
- **PecProvider**: SaleServiceSoapClient, ConfirmServiceSoapClient
- **BehPardakhtProvider**: PaymentGatewayClient

### Settlement Integration Pattern

**Post-Verification Settlement** (AsanPardakht and BehPardakht):
- Settlement automatically called after successful transaction verification
- Implemented in `VerifyTransactionQueryHandler` with provider-specific conditional logic
- **BehPardakht**: Uses TrackId and ReferenceNumber for settlement
- **AsanPardakht**: Uses ProviderTrackerId for settlement (added 2025-10-17)
- Settlement status updates IPGTransaction.Status (9 = SettlementSucceeded, 10 = SettlementFailed)
- PredictedSettlementDateTime calculated based on settlement status and time thresholds:
  - Successful settlement (status 9): 23:45 time threshold
  - Failed settlement (status 10): 20:40 time threshold

Example usage:
```csharp
return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
{
    using (var soapClient = new SoapClient(...))
    {
        var response = await soapClient.CallAsync(...);
        return processedResult;
    }
}, "ServiceName");
```

**Optional: Enhanced SOAP Logging with Request/Response Body**

For detailed logging including request/response bodies on timeout, inject `ISoapLoggingWrapper`:

```csharp
public class MyProvider(IPollyPolicyService pollyService, ISoapLoggingWrapper soapLogger)
{
    public async Task<Response> CallSoapAsync(Request request)
    {
        return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
        {
            return await _soapLogger.ExecuteWithLoggingAsync(
                serviceName: "MyService",
                operationName: "MyOperation",
                request: request,
                soapCall: async () =>
                {
                    using var client = new SoapClient(...);
                    return await client.CallAsync(request);
                });
        }, "MyProvider");
    }
}
```

### Handled Exceptions

Polly automatically retries on:
- **HTTP**: Transient errors (5xx), 408 Request Timeout, 429 Too Many Requests, TimeoutException
- **SOAP**: EndpointNotFoundException, CommunicationException, TimeoutException, ServerTooBusyException

### Logging

All retry attempts, circuit breaker state changes, and timeouts are automatically logged with:
- Retry count and delay duration
- Exception details
- Service/policy name
- Circuit breaker state transitions (Open, Half-Open, Closed)

**Enhanced Timeout Logging**:
- `PollyLoggingHandler`: Automatically logs **complete HTTP Request and Response body** on timeout/error for all named HttpClients
- `SoapLoggingWrapper`: Optional wrapper to log **complete SOAP Request and Response** on timeout/error
- All logs include:
  - Request ID for tracing
  - Duration in milliseconds
  - Full request body (truncated at 3000 chars for errors)
  - Full response body (truncated at 2000-3000 chars)
  - Exception type and message

Example timeout log:
```
[abc12345] HTTP Request TIMEOUT after 30021ms | Request: POST https://api.example.com/endpoint | Request Body: {"amount":1000,...}
```

## Event Storming

Event storming session board available at: https://miro.com/app/board/uXjVOZIABVo=/
