# Domain-Driven Design Architecture

## Layer Structure

This project follows a strict layered DDD architecture with enforced dependencies to maintain clean architecture principles.

### Core.Domain Layer
**Purpose:** Contains pure domain logic without external dependencies
- **Abstractions/** - Base classes for entities, aggregate roots, value objects, domain events, and repository interfaces
- **Events/** - Domain events that represent state changes in the domain
- **Dependencies:** None (Zero external dependencies)
- **Who can depend on this:** Everyone
- **Who this depends on:** Nobody

**Key Concepts:**
- `Entity<TId>` - Base class for mutable domain objects with identity
- `AggregateRoot<TId>` - Cluster of entities treated as a single unit
- `ValueObject` - Immutable objects compared by value
- `DomainEvent` - Events representing domain facts
- `IRepository<T, TId>` - Interface for persistence abstraction
- `IUnitOfWork` - Interface for coordinating changes

### Core.Application Layer
**Purpose:** Application use cases, commands, queries, and orchestration
- **Abstractions/** - CQRS interfaces (ICommand, IQuery, ICommandHandler, IQueryHandler)
- **Features/** - Feature-based organization of use cases
- **DTOs/** - Data transfer objects for application contracts
- **Extensions/** - IoC setup and configuration
- **Dependencies:** Core.Domain, MediatR
- **Who can depend on this:** Infrastructure, BlazorUI (Presentation)
- **Who this depends on:** Core.Domain only

**Key Concepts:**
- `ICommand` / `ICommand<T>` - Write operations
- `IQuery<T>` - Read operations
- `IMediator` - Central dispatcher for commands/queries
- Command/Query Handlers - Orchestrate domain and infrastructure operations

### Core.Infrastructure Layer
**Purpose:** External concerns - data access, external services, third-party integrations
- **Persistence/** - Database implementations, repositories, UnitOfWork
- **External/** - External service integrations (APIs, messaging, etc.)
- **Dependencies:** Core.Domain, Core.Application
- **Who can depend on this:** BlazorUI (Presentation)
- **Who this depends on:** Core layers only

**Key Concepts:**
- Repository implementations
- UnitOfWork implementation
- Database context setup
- External service adapters

### Shared Layer
**Purpose:** Cross-cutting utilities and common code
- **Utilities/** - Helpers, extensions, utilities used across layers
- **Dependencies:** None
- **Who can depend on this:** Everyone
- **Who this depends on:** Nobody

### BlazorUI Layer (Presentation)
**Purpose:** User interface - Blazor components and page interactions
- **Components/** - Razor components
- **Pages/** - Razor pages
- **Dependencies:** Core.Application, Infrastructure, Shared
- **Who can depend on this:** Nobody (Top layer)
- **Who this depends on:** All lower layers

## Dependency Rules (Strict)

✅ **ALLOWED:**
- Core.Domain → (nothing)
- Core.Application → Core.Domain
- Infrastructure → Core.Application, Core.Domain
- BlazorUI → Infrastructure, Core.Application, Shared
- BlazorUI → Core.Domain (only through Core.Application)
- Shared → (nothing)

❌ **FORBIDDEN:**
- Core.Domain depending on anything
- Core.Application depending on Infrastructure
- Core.Application depending on BlazorUI
- Infrastructure depending on BlazorUI
- Circular dependencies of any kind

## CQRS Pattern

### Commands (State-changing operations)
```csharp
public record CreateUserCommand(string Name, string Email) : ICommand<Guid>;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly Core.Domain.Abstractions.IUnitOfWork _unitOfWork;

    public async Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Business logic here
        // Use domain entities
        // Use repositories from Core.Domain
    }
}
```

### Queries (Read operations)
```csharp
public record GetUserQuery(Guid UserId) : IQuery<UserDTO>;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDTO>
{
    private readonly IUserRepository _userRepository;

    public async Task<UserDTO> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Read-only operations
        // Map domain to DTOs
    }
}
```

## Creating New Features

1. **Define Domain Model** in `Core.Domain`
   - Create entities, value objects, aggregate roots
   - Define events

2. **Create Use Cases** in `Core.Application`
   - Create commands/queries
   - Create handlers
   - Define DTOs

3. **Implement Persistence** in `Infrastructure`
   - Create repository implementations
   - Setup database mappings

4. **Build UI** in `BlazorUI`
   - Create components
   - Use mediator to dispatch commands/queries

## Enforcing Architecture

### Build-time Checks
Use InternalsVisibleTo attributes to enforce layer boundaries at compile time.
See `.csproj` files for InternalsVisibleTo configurations.

### Runtime Checks
- Dependency injection container setup prevents misuse
- Interfaces segregation enforces proper dependencies
- No static references across layers

## Example: User Feature

```
Core.Domain/
  ├─ Abstractions/
  │  └─ IUserRepository.cs
  └─ Entities/
     └─ User.cs (AggregateRoot)

Core.Application/
  ├─ DTOs/
  │  └─ UserDTO.cs
  └─ Features/User/
     ├─ CreateUser/
     │  ├─ CreateUserCommand.cs
     │  └─ CreateUserCommandHandler.cs
     └─ GetUser/
        ├─ GetUserQuery.cs
        └─ GetUserQueryHandler.cs

Infrastructure/
  └─ Persistence/
     └─ UserRepository.cs

BlazorUI/
  └─ Pages/
     └─ Users/
        └─ UserList.razor
```

## References

- Domain-Driven Design (Evans)
- Clean Architecture (Robert C. Martin)
- CQRS Pattern
- Event-Driven Architecture
