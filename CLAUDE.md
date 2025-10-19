# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CPG (Charisma Payment Gateway) is a .NET 8 enterprise payment gateway system implementing Clean Architecture with Domain-Driven Design (DDD) and CQRS patterns. It integrates with multiple payment providers and handles payment processing, direct debits, and financial transactions.

## Architecture

The solution follows Clean Architecture with clear separation:
- **Domain Layer** (`CPG.Domain/`) - Core business logic, entities, aggregates
- **Application Layer** (`CPG.Application/`) - Use cases, CQRS commands/queries, MediatR handlers
- **Infrastructure Layer** (`CPG.Infrastructure/`) - External service integrations, providers
- **Persistence Layer** (`CPG.Infrastructure.Persistence/`) - EF Core, repositories, migrations
- **API Layer** (`CPG.API/`) - REST API controllers, GraphQL endpoints

### Key Patterns
- **CQRS with MediatR** - Separate read/write models with `WriteDbContext` and `ReadDbContext`
- **Specification Pattern** - Complex queries using Ardalis.Specification
- **Repository Pattern** - Generic repositories with specification support
- **Domain Aggregates** - PaymentRequest, Company, Application, User, etc.

## Common Development Commands

**⚠️ CRITICAL: NEVER run `dotnet build` or any build commands in this project. The user will test and build manually.**

### Build and Run
```bash
# Build the solution (DO NOT RUN THIS - USER WILL BUILD MANUALLY)
dotnet build

# Run the API locally
dotnet run --project src/CPG.API/CPG.API.csproj

# Run with Docker Compose (includes SQL Server)
docker-compose up

# Run tests
dotnet test
```

### Database Migrations
```bash
# Add a new migration (run from solution root)
dotnet ef migrations add <MigrationName> --context WriteDbContext --project src/CPG.Infrastructure.Persistence --startup-project src/CPG.API --output-dir Migrations

# Update database
dotnet ef database update --context WriteDbContext --project src/CPG.Infrastructure.Persistence --startup-project src/CPG.API
```

For Package Manager Console in Visual Studio:
- Set default project to `CPG.Infrastructure.Persistence`
- Run: `Add-Migration <name> -Context WriteDbContext -o Migrations`
- Run: `Update-Database -Context WriteDbContext`

### Testing
```bash
# Run unit tests only
dotnet test --filter FullyQualifiedName~CPG.Domain.Tests.Unit

# Run architecture tests
dotnet test --filter FullyQualifiedName~CPG.Architecture.Tests

# Run a specific test
dotnet test --filter "FullyQualifiedName~<TestClassName>.<MethodName>"
```

## Core Business Flows

### Payment Request Flow
1. **Create Payment Request** - `PaymentRequests/Commands/CreatePaymentRequest/`
2. **Process via Provider** - Factory pattern selects provider (CharisPay, AsanPardakht, Vandar)
3. **Handle Callback** - `IPGResult/Commands/` processes provider responses
4. **Update Status** - Domain events trigger status updates

### Adding New Features

When implementing new business features:
1. Start in **Domain** - Create aggregate/entity in `AggregateModels/`
2. Add **Use Cases** - Create command/query handlers in `Application/UseCases/`
3. Add **Repository** if needed in `Infrastructure.Persistence/Repositories/`
4. Add **Controller** endpoint in `API/Controllers/v1/`
5. Add **EF Configuration** in `Infrastructure.Persistence/DbContexts/EntityConfigurations/`

### External Service Integration

Payment providers are integrated via:
- **Provider Interfaces** in `Domain/SharedKernel/Communication/`
- **Provider Implementations** in `Infrastructure/Providers/`
- **Factory Pattern** for provider selection (`IIpgFactory`, `IDirectDebitFactory`)

Current providers:
- CharisPay (Primary payment)
- AsanPardakht (IPG)
- Vandar (Direct debit)
- CharismaCard
- NeoBank

## Logging Standards

**CRITICAL: This project uses a custom logging system. NEVER use `ILogger<T>` directly.**

### Mandatory Logging Pattern

All logging MUST be done through `ILogService` with structured `CallLogModel`:

```csharp
// ✅ CORRECT - Use ILogService with CallLogModel
private readonly ILogService _logService;

var callLog = CallLogModel.CreateError(
    serviceName: "ServiceName",
    providerName: "ProviderName",
    requestUri: "endpoint",
    requestBody: jsonRequest,
    responseBody: jsonResponse,
    exception: exc,
    serviceType: Enums.ServiceType.SomeType,
    providerType: Enums.ProviderTypeInLog.SomeProvider,
    auditType: Enums.AuditType.Provider,
    userId: userId
);
_logService.LogError(callLog);

// ❌ WRONG - DO NOT use ILogger directly
private readonly ILogger<MyClass> _logger;  // FORBIDDEN
_logger.LogError(exc, "Error message");     // FORBIDDEN
```

### Logging Methods Available

**For HTTP/REST API Calls:**
- `_logService.AddServiceCallLog<TBody>(HttpProviderRequest, HttpResponseMessage, string)`
- Automatically creates `CallLogModel` with proper formatting

**For SOAP Service Calls:**
- `_logService.AddSoapCallLog<TRequest, TResponse>(request, response, serviceName, status, message)`
- `_logService.AddSoapTimeoutLog<TRequest>(request, serviceName, exception, durationMs)`

**For Custom Structured Logs:**
- `_logService.LogInformation(CallLogModel)` - Success/info logs
- `_logService.LogWarning(CallLogModel)` - Warning logs
- `_logService.LogError(CallLogModel)` - Error logs
- `_logService.LogDebug(CallLogModel)` - Debug logs

### CallLogModel Factory Methods

Use these static methods to create properly formatted logs:

```csharp
// Success logs
var successLog = CallLogModel.CreateSuccess(
    serviceName: "ServiceName",
    providerName: "ProviderName",
    requestUri: "/api/endpoint",
    requestBody: requestJson,
    responseBody: responseJson,
    serviceType: Enums.ServiceType.SomeType,
    providerType: Enums.ProviderTypeInLog.SomeProvider,
    auditType: Enums.AuditType.Provider,
    userId: userId,
    responseStatusCode: 200
);

// Error logs
var errorLog = CallLogModel.CreateError(
    serviceName: "ServiceName",
    providerName: "ProviderName",
    requestUri: "/api/endpoint",
    requestBody: requestJson,
    responseBody: errorMessage,
    exception: exc,
    serviceType: Enums.ServiceType.SomeType,
    providerType: Enums.ProviderTypeInLog.SomeProvider,
    auditType: Enums.AuditType.Provider,
    userId: userId
);
```

### Why This Pattern?

1. **Consistent Format**: All logs follow the same structure for Elasticsearch indexing
2. **PII Protection**: Automatic masking of sensitive data (passwords, keys, etc.)
3. **Traceability**: Built-in correlation IDs, user tracking, and audit trails
4. **Performance**: Optimized for high-volume financial transaction logging
5. **Compliance**: Meets audit and regulatory requirements

### Log Properties Automatically Captured

When using `ILogService`, these are automatically populated:
- Correlation ID (from HTTP context)
- User ID, Application ID, Company ID
- IP Address and User Agent
- Start/End timestamps and duration
- Request/Response headers (with PII masking)

**NEVER bypass ILogService for application logging.**

## Retry Policies

**⚠️ IMPORTANT: All retry logic uses ONLY Polly policies configured in appsettings.json. Manual retry counters are forbidden.**

### Unified Configuration

**One single retry policy for ALL services** (HTTP and SOAP) configured in `appsettings.json` under `Infrastructure:Polly`:

```json
"Polly": {
  "Retry": {
    "MaxRetryAttempts": 3,
    "BaseDelaySeconds": 1,
    "UseExponentialBackoff": true,
    "UseJitter": true,
    "MaxJitterMilliseconds": 1000
  },
  "Timeout": {
    "SoapTimeoutSeconds": 45
  }
}
```

**Configuration Properties:**
- `MaxRetryAttempts`: Number of retry attempts (default: 3)
- `BaseDelaySeconds`: Base delay between retries (default: 1 second)
- `UseExponentialBackoff`:
  - `true` = exponential backoff (1s, 2s, 4s...)
  - `false` = linear backoff (1s, 2s, 3s...)
- `UseJitter`: Add random jitter to prevent thundering herd (default: true)
- `MaxJitterMilliseconds`: Maximum random jitter in milliseconds (default: 1000ms)

### HTTP REST API Calls

All HTTP clients (named and unnamed) use the **same unified retry policy**:

```csharp
// Automatically applied to all HTTP clients in DependencyInjection.cs
services.AddHttpClient()
    .ConfigureHttpClientDefaults(builder =>
    {
        builder.AddPolicyHandler(PollyRetryConfiguration.GetHttpRetryPolicy(policyConfig));
    });
```

**Services using this policy:**
- CharisPay
- IdpClient
- NeoBank
- AsanPardakht
- CharismaCard
- All unnamed HttpClient instances (via HttpProvider)

### SOAP Service Calls

SOAP services use `IPollyPolicyService` with the **same unified retry configuration**:

```csharp
// Inject IPollyPolicyService
private readonly IPollyPolicyService _pollyPolicyService;

// Wrap SOAP calls with retry + timeout policy
return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
{
    using var soapClient = new SomeServiceSoapClient(...);
    var response = await soapClient.SomeMethodAsync(request);

    // Log the call
    _logService.AddSoapCallLog(request, response, "SomeMethod", status, message);

    return response;
}, "ServiceName.MethodName");
```

### Changing Retry Behavior

To adjust retry behavior for **all services at once**, simply edit `appsettings.json`:

```json
// Example: More aggressive retries with longer delays
"Retry": {
  "MaxRetryAttempts": 5,
  "BaseDelaySeconds": 2,
  "UseExponentialBackoff": true,
  "UseJitter": true,
  "MaxJitterMilliseconds": 2000
}

// Example: Conservative linear retries
"Retry": {
  "MaxRetryAttempts": 2,
  "BaseDelaySeconds": 1,
  "UseExponentialBackoff": false,
  "UseJitter": false,
  "MaxJitterMilliseconds": 0
}
```

No code changes required - configuration is applied at runtime.

## Configuration

### Development Settings
Key configuration in `appsettings.Development.json`:
- **SQL Server** - Connection string in `ConnectionStrings:CPGConnectionString`
- **Redis Sentinel** - Cache configuration in `Redis` section
- **RabbitMQ** - Message queue in `Infrastructure:RabbitMQ`
- **MinIO** - File storage in `Infrastructure:Minio`
- **JWT** - Authentication in `JwtConfig`

### Environment Variables
Docker Compose uses environment variables for:
- `ConnectionStrings__CPGConnectionString`
- `JwtConfig__Secret`
- Redis sentinel endpoints
- External service URLs

## Key Dependencies

### Core Framework
- **.NET 8.0** - Target framework
- **MediatR** (12.2.0) - CQRS implementation
- **Entity Framework Core** (8.0.0) - ORM
- **FluentValidation** (11.9.2) - Request validation
- **Mapster** (7.4.0) - Object mapping

### Infrastructure
- **Redis** with Sentinel - Caching (StackExchange.Redis 2.6.122)
- **RabbitMQ** - Message queue (MassTransit 8.0.1)
- **MinIO** - S3-compatible storage
- **Serilog** - Structured logging with Elasticsearch sink

### API
- **Swagger/OpenAPI** - API documentation
- **GraphQL** (Hot Chocolate 13.8.1) - Complex queries at `/api/graphql`
- **API Versioning** - Header-based versioning

## Troubleshooting

### Common Issues

1. **Migration Failures**
   - Ensure `CPG.Infrastructure.Persistence` is set as default project
   - Check connection string in appsettings
   - Verify SQL Server is running (docker-compose includes it)

2. **Redis Connection**
   - System uses Sentinel mode, not standalone Redis
   - Check sentinel endpoints in configuration
   - Can disable with `"Redis:Enable": "false"`

3. **External Service Errors**
   - HttpProvider uses Polly for retry policies
   - Check provider URLs in `Infrastructure` configuration
   - Verify API keys/tokens for each provider

4. **Build Errors with Custom NuGet**
   - Repository uses private NuGet feed
   - Check `nuget.config` for feed URLs
   - May need authentication for Charisma artifact repository

## Project Structure for Navigation

Key directories for common tasks:
- **Business Logic** → `src/CPG.Domain/AggregateModels/`
- **API Endpoints** → `src/CPG.API/Controllers/v1/`
- **Command Handlers** → `src/CPG.Application/UseCases/*/Commands/`
- **Query Handlers** → `src/CPG.Application/UseCases/*/Queries/`
- **Database Configs** → `src/CPG.Infrastructure.Persistence/DbContexts/EntityConfigurations/`
- **External Services** → `src/CPG.Infrastructure/Providers/`
- **Migrations** → `src/CPG.Infrastructure.Persistence/Migrations/`