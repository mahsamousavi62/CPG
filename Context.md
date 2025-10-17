# Project Context: CPG (Payment Gateway)

## Project Overview

- **Version**: ContextKit 0.1.0
- **Setup Date**: 2025-10-17
- **Components**: 6 .NET projects (1 API, 3 infrastructure, 1 domain, 1 application) + 3 test projects
- **Workspace**: None (standalone project)
- **Primary Tech Stack**: .NET 8.0, C#, ASP.NET Core, Entity Framework Core, Clean Architecture
- **Development Guidelines**: No specific guidelines copied (C# guidelines not available in ContextKit templates)

## Component Architecture

**Project Structure**:

```
📁 CPG Payment Gateway
├── 🖥️ CPG.API (Web API) - REST API presentation layer - ASP.NET Core 8.0 - ./src/CPG.API
├── 📦 CPG.Application (Class Library) - Application business rules and use cases (CQRS) - .NET 8.0 - ./src/CPG.Application
├── 📦 CPG.Domain (Class Library) - Core domain models and business logic (DDD) - .NET 8.0 - ./src/CPG.Domain
├── 📦 CPG.Infrastructure (Class Library) - External service integrations (Kafka, RabbitMQ, Polly) - .NET 8.0 - ./src/CPG.Infrastructure
├── 📦 CPG.Infrastructure.Persistence (Class Library) - Data access layer (EF Core, GraphQL) - .NET 8.0 - ./src/CPG.Infrastructure.Persistence
├── 📦 CPG.Infrastructure.Authorization (Class Library) - Authentication and authorization (JWT) - .NET 8.0 - ./src/CPG.Infrastructure.Authorization
└── 🔧 Tests
    ├── CPG.Domain.Tests.Unit (xUnit) - Domain unit tests - .NET 8.0 - ./tests/CPG.Domain.Tests.Unit
    ├── CPG.Architecture.Tests (xUnit) - Architecture validation tests (NetArchTest) - .NET 8.0 - ./tests/CPG.Architecture.Tests
    └── CPG.Tests.Base (Class Library) - Shared test utilities - .NET 8.0 - ./tests/CPG.Tests.Base
```

**Component Summary**:
- **6 .NET 8.0 projects** - Following Clean Architecture with CQRS and DDD patterns
- **3 test projects** - Unit tests and architecture validation using xUnit
- **Dependencies**: Entity Framework Core 8.0, MediatR 12.x, FluentValidation 11.x, Polly 8.x, HotChocolate 13.x, Serilog

---

## Component Details

### CPG Solution - Unified .NET Project

**Location**: `./` (project root)
**Purpose**: Payment gateway REST API with Clean Architecture implementation handling payment requests, IPG integrations, direct debit, and various payment methods
**Tech Stack**: .NET 8.0 (SDK 9.0.110 installed), C#, ASP.NET Core, Entity Framework Core 8.0

**File Structure**:
```
CPG/
├── src/
│   ├── CPG.API/              # REST API presentation layer
│   ├── CPG.Application/      # Application business rules (CQRS, MediatR, FluentValidation)
│   ├── CPG.Domain/           # Domain models (DDD, Aggregate Roots, Domain Events)
│   ├── CPG.Infrastructure/   # External integrations (Kafka, RabbitMQ, Polly, Minio)
│   ├── CPG.Infrastructure.Persistence/  # Data access (EF Core, GraphQL, Redis)
│   └── CPG.Infrastructure.Authorization/ # Auth (JWT, middleware)
├── tests/
│   ├── CPG.Domain.Tests.Unit/      # Domain unit tests
│   ├── CPG.Architecture.Tests/     # Architecture validation
│   └── CPG.Tests.Base/             # Shared test utilities
├── CPG.sln                   # Solution file
├── nuget.config              # NuGet configuration
├── docker-compose.yml        # Docker orchestration
└── azure-pipeline.yaml       # CI/CD pipeline configuration
```

**Key Dependencies**:
- **Entity Framework Core 8.0** - SQL Server data access with CQRS read/write separation
- **MediatR 12.x** - CQRS request handling with pipeline behaviours
- **FluentValidation 11.x** - Request validation with Chain of Responsibility pattern
- **Polly 8.x** - Resilience policies (retry, circuit breaker, timeout)
- **HotChocolate 13.x** - GraphQL server for queries
- **Mapster 7.x** - Object-to-object mapping
- **Serilog** - Structured logging (Elasticsearch, Console, File sinks)
- **Ardalis.Specification** - Repository pattern with specifications
- **xUnit** - Testing framework
- **NetArchTest.Rules** - Architecture testing

**Development Commands**:
```bash
# Clean solution
dotnet clean CPG.sln

# Restore dependencies (uses nuget.config)
dotnet restore CPG.sln --configfile nuget.config

# Build solution (validated during setup)
dotnet build CPG.sln --configuration Release

# Run all tests (validated during setup)
dotnet test CPG.sln --configuration Release

# Run unit tests only
dotnet test CPG.sln --filter FullyQualifiedName~Tests.Unit --configuration Release

# Run architecture tests
dotnet test tests/CPG.Architecture.Tests/CPG.Architecture.Tests.csproj

# Run specific test project
dotnet test tests/CPG.Domain.Tests.Unit/CPG.Domain.Tests.Unit.csproj --configuration Release

# Run API locally
dotnet run --project src/CPG.API/CPG.API.csproj

# Watch mode for development
dotnet watch --project src/CPG.API/CPG.API.csproj

# Docker commands
docker-compose up --build        # Build and run
docker-compose up -d            # Run in detached mode
docker-compose logs -f          # View logs
docker-compose down             # Stop containers
```

**Database Commands**:
```bash
# Entity Framework migrations (from Package Manager Console)
Add-Migration <migration_name> -Context WriteDbContext -o Migrations
Update-Database -Context WriteDbContext
```

**Code Style** (detected):
- No .editorconfig or formatter configurations found in project
- Standard C# conventions (ImplicitUsings enabled, Nullable reference types enabled)
- Documentation file generation enabled for API project

---

## Development Environment

**Requirements**:
- .NET 8.0 SDK (currently using SDK 9.0.110 which is compatible)
- SQL Server 2019 (for production, via Docker)
- Docker and Docker Compose (for containerized deployment)
- Entity Framework Core CLI tools (for migrations)

**Build Tools**:
- dotnet CLI 9.0.110
- MSBuild (via .NET SDK)
- NuGet (configured via nuget.config)
- Docker
- Azure DevOps Pipelines (CI/CD)

**Formatters**:
- No custom formatters configured
- Standard C# formatting via IDE/editor settings
- Consider adding .editorconfig for team consistency

## Development Guidelines

**Applied Guidelines**: None (C# guidelines not yet available in ContextKit templates)
- ContextKit currently provides Swift and SwiftUI guidelines
- Future versions may include C#/.NET specific guidelines
- Refer to CLAUDE.md for project-specific architectural patterns and conventions

**Project Conventions** (from CLAUDE.md):
- Clean Architecture with clear layer separation
- CQRS pattern for command/query separation
- DDD patterns (Entities, Value Objects, Domain Events, Aggregate Roots)
- Repository pattern with Specification pattern
- MediatR for request handling with pipeline behaviours
- FluentValidation for request validation
- Chain of Responsibility for complex validations

## Constitutional Principles

**Core Principles** (adapted for backend API):
- ✅ Security-first design (authentication, authorization, data protection)
- ✅ Privacy by design (minimal data collection, GDPR compliance, secure payment handling)
- ✅ Localizability from day one (externalized strings via Resource.resx files, multi-language support)
- ✅ Code maintainability (Clean Architecture, testable code, comprehensive documentation)
- ✅ API best practices (RESTful conventions, proper status codes, versioning)
- ✅ Resilience by design (Polly policies for retry, circuit breaker, timeout)
- ✅ Observability (comprehensive logging via Serilog, distributed tracing)

**Workspace Inheritance**: None - using global defaults adapted for payment gateway context

## ContextKit Workflow

**Systematic Feature Development**:
- `/ctxk:plan:1-spec` - Create business requirements specification (prompts interactively)
- `/ctxk:plan:2-research-tech` - Define technical research, architecture and implementation approach
- `/ctxk:plan:3-steps` - Break down into executable implementation tasks

**Development Execution**:
- `/ctxk:impl:start-working` - Continue development within feature branch (requires completed planning phases)
- `/ctxk:impl:commit-changes` - Auto-format code and commit with intelligent messages

**Backlog Management**:
- `/ctxk:bckl:add-idea` - Add new feature ideas to Ideas-Inbox.md
- `/ctxk:bckl:add-bug` - Add bugs to Bugs-Backlog.md

**Quality Assurance**: Automated agents available (though primarily Swift-focused)
**Project Management**: All validated build/test commands documented above for immediate use

## Development Automation

**Hooks Configured**:
- **PostToolUse** (Edit/Write/MultiEdit): AutoFormat.sh - Automatic code formatting after file changes
- **SessionStart**: VersionStatus.sh - Display ContextKit version and status on session start

**Status Line**:
- Custom status line showing 5-hour usage tracking and context progress
- Configured for Max 20x plan (configurable in .claude/settings.json)
- Real-time monitoring with colored progress bars

**Quality Agents Available** (note: primarily Swift-focused):
- `build-project` - Execute builds with validation
- `check-accessibility` - Accessibility validation (UI-focused)
- `check-localization` - Localization validation
- `check-error-handling` - Error handling patterns
- `check-modern-code` - Code modernization checks
- `check-code-debt` - Technical debt cleanup

## Configuration Hierarchy

**Inheritance**: None (standalone project)

**Configuration Locations**:
- **Project Context**: This file (Context.md) - ContextKit workflow and component details
- **Project Documentation**: CLAUDE.md - Comprehensive architectural patterns and conventions
- **Settings**: .claude/settings.json - Permissions, hooks, model, status line

**Override Precedence**: Context.md configurations supplement CLAUDE.md documentation

## CI/CD Pipeline

**Azure DevOps Pipeline Stages** (azure-pipeline.yaml):
1. Clean and Restore
2. Build (Release configuration)
3. Run Unit Tests
4. Run Integration Tests
5. Publish artifacts
6. Docker build and push (branch-specific):
   - `develop` → cpg/dev/cpg-api-backend
   - `stage` → cpg/stage/cpg-api-backend
   - `master` → cpg/prod/cpg-api-backend

## Additional Resources

- **Event Storming Board**: https://miro.com/app/board/uXjVOZIABVo=/
- **Architecture Documentation**: See CLAUDE.md for detailed architectural patterns
- **API Documentation**: Swagger/OpenAPI available when running API locally

---
*Generated by ContextKit 0.1.0 with comprehensive .NET project analysis. Manual edits preserved during updates.*