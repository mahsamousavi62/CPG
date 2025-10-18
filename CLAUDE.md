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

### Build and Run
```bash
# Build the solution
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