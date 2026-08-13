# DDD Quick Reference

## Project Structure at a Glance

### Core.Domain
**Purpose:** Pure business logic, no external dependencies
- Entities, AggregateRoots, ValueObjects
- Domain Events
- Repository Interfaces
- Business rules enforcement

### Core.Application
**Purpose:** Use case orchestration via CQRS
- Commands & Queries
- Handlers
- DTOs
- Application Exceptions

### Core.Infrastructure
**Purpose:** External concerns and persistence
- DbContext & Migrations
- Repository Implementations
- Unit of Work
- External Service Adapters

### BlazorUI (ProjectZero)
**Purpose:** User interface
- Razor Components
- Pages
- Uses Mediator to dispatch commands/queries

## Creating a New Domain Feature - Quick Steps

```bash
# 1. Create aggregate in Core.Domain
Core.Domain/Entities/MyEntity.cs

# 2. Create repository interface in Core.Domain
Core.Domain/Repositories/IMyRepository.cs

# 3. Create DTO in Core.Application
Core.Application/DTOs/MyDTO.cs

# 4. Create Command/Query in Core.Application
Core.Application/Features/MyFeature/Command/MyCommand.cs

# 5. Create Handler in Core.Application
Core.Application/Features/MyFeature/Command/MyCommandHandler.cs

# 6. Implement repository in Infrastructure
Infrastructure/Persistence/Repositories/MyRepository.cs

# 7. Register in IoC (Core.Infrastructure/Extensions/ServiceCollectionExtensions.cs)
services.AddScoped<IMyRepository, MyRepository>();

# 8. Use in Blazor Component
@inject IMediator Mediator
await Mediator.SendAsync(new MyCommand(...));
```

## Key Dependencies

- **MediatR** - CQRS pattern implementation
- **Entity Framework Core** - ORM
- **Microsoft.Extensions.DependencyInjection** - IoC container

## Build & Test

```bash
# Restore and build
dotnet build

# Run project
dotnet run --project ProjectZero

# Build specific project
dotnet build Core.Domain
```

## Important: Never Break These Rules

✅ **DO:**
- Put business logic in Domain layer (entities, aggregates)
- Use Commands for writes, Queries for reads
- Dispatch via Mediator in Blazor components
- Return DTOs from handlers, not domain entities
- Keep Domain layer dependency-free

❌ **DON'T:**
- Reference Infrastructure from Domain or Application
- Use domain entities directly in UI (use DTOs)
- Mix Commands and Queries logic
- Put business rules outside entities
- Create circular dependencies

## File Naming Conventions

- **Entities:** `MyEntity.cs`
- **Repositories:** `IMyRepository.cs` (interface), `MyRepository.cs` (impl)
- **Commands:** `MyCommand.cs`
- **Handlers:** `MyCommandHandler.cs`, `MyQueryHandler.cs`
- **DTOs:** `MyDTO.cs`
- **Events:** `MyDomainEvent.cs`

## Example: User Feature

```
Core.Domain/
  ├─ Entities/User.cs (AggregateRoot)
  ├─ Repositories/IUserRepository.cs
  └─ Events/UserCreatedEvent.cs

Core.Application/
  ├─ DTOs/UserDTO.cs
  └─ Features/Users/
     ├─ CreateUser/
     │  ├─ CreateUserCommand.cs
     │  └─ CreateUserCommandHandler.cs
     └─ GetUser/
        ├─ GetUserQuery.cs
        └─ GetUserQueryHandler.cs

Core.Infrastructure/
  └─ Persistence/
     ├─ UserRepository.cs
     └─ ApplicationDbContext.cs

BlazorUI/
  └─ Components/UserForm.razor
```

## Building from CLI

```bash
# Full build with detailed errors
dotnet build -v normal

# Build only Core.Domain
dotnet build Core.Domain/Core.Domain.csproj

# Clean and rebuild
dotnet clean && dotnet build

# Build in Release mode
dotnet build -c Release
```

## References

- See `DDD_ARCHITECTURE.md` for detailed documentation
- See `README.md` for project overview
- Check example handlers in `Core.Application/Features/Users/`
- Review domain entity in `Core.Domain/Entities/User.cs`
