# Domain-Driven Design (DDD) Architecture Implementation

## Overview

Your ProjectZero project now implements a proper **Domain-Driven Design (DDD)** architecture with enforced layer dependencies to maintain clean, scalable, and maintainable code.

## ✅ What's Been Implemented

### 1. **Layered Architecture**
The solution is organized into 4 main layers:

```
┌─────────────────────────────────────┐
│  ProjectZero (Presentation)  │ (UI Layer - No dependencies on other layers except lower)
├─────────────────────────────────────┤
│  Infrastructure                      │ (Data Access, External Services)
├─────────────────────────────────────┤
│  Core.Application                    │ (Use Cases, Commands, Queries, CQRS)
├─────────────────────────────────────┤
│  Core.Domain                         │ (Business Logic, Entities, Aggregates)
└─────────────────────────────────────┘
     + Shared (Cross-cutting Utilities)
```

### 2. **Core Domain Layer** (`Core.Domain`)
- **Base Classes:**
  - `Entity<TId>` - Mutable objects with identity
  - `AggregateRoot<TId>` - Clusters of entities treated as a single unit
  - `ValueObject` - Immutable value comparisons
  - `DomainEvent` - Events representing domain facts

- **Repositories:**
  - `IRepository<T, TId>` - Abstract persistence interface
  - `IUnitOfWork` - Coordinate changes

- **Example Implementation:**
  - `User` aggregate root with domain events
  - `IUserRepository` interface

### 3. **Application Layer** (`Core.Application`)
- **CQRS Pattern:**
  - `ICommand` / `ICommand<T>` - Write operations
  - `IQuery<T>` - Read operations
  - `ICommandHandler<TCommand, TResponse>`
  - `IQueryHandler<TQuery, TResponse>`
  - `IMediator` - Central dispatcher

- **DTOs:** Data Transfer Objects for API contracts
  
- **Exception Handling:**
  - `ApplicationException` - Base exception
  - `EntityNotFoundException` - Entity not found
  - `ValidationException` - Validation errors

- **Example Handlers:**
  - `CreateUserCommand` → `CreateUserCommandHandler`
  - `GetUserQuery` → `GetUserQueryHandler`

### 4. **Infrastructure Layer** (`Infrastructure`)
- **Persistence:**
  - `ApplicationDbContext` - EF Core DbContext
  - `UserRepository` - Repository implementation
  - `UnitOfWork` - Transaction coordinator

- **Current Database:** In-Memory (ready for SQL Server or other providers)

### 5. **Dependency Injection Setup**
- `Core.Application.Extensions.ServiceCollectionExtensions` - Registers MediatR and app services
- `Infrastructure.Extensions.ServiceCollectionExtensions` - Registers repositories and DbContext
- `Program.cs` - Wired up in startup

## 📋 Project Structure

```
ProjectZero/
├── Core.Domain/                      (Pure business logic)
│   ├── Abstractions/                 (Base classes)
│   │   ├── Entity.cs
│   │   ├── AggregateRoot.cs
│   │   ├── ValueObject.cs
│   │   ├── DomainEvent.cs
│   │   ├── IRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Entities/                     (Domain objects)
│   │   └── User.cs (with events)
│   └── Repositories/
│       └── IUserRepository.cs
│
├── Core.Application/                 (Use cases)
│   ├── Abstractions/                 (CQRS interfaces)
│   │   ├── ICommand.cs
│   │   ├── ICommandHandler.cs
│   │   ├── IQuery.cs
│   │   ├── IQueryHandler.cs
│   │   ├── IMediator.cs
│   │   └── ApplicationExceptions.cs
│   ├── DTOs/
│   │   └── UserDTO.cs
│   ├── Features/
│   │   └── Users/
│   │       ├── CreateUser/
│   │       │   ├── CreateUserCommand.cs
│   │       │   └── CreateUserCommandHandler.cs
│   │       └── GetUser/
│   │           ├── GetUserQuery.cs
│   │           └── GetUserQueryHandler.cs
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs
│
├── Core.Infrastructure/              (External concerns)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── UnitOfWork.cs
│   │   └── Repositories/
│   │       └── UserRepository.cs
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs
│
├── Shared/                           (Cross-cutting utilities)
│   └── Utilities/
│
├── ProjectZero/               (Presentation - UI)
│   ├── Components/
│   ├── Pages/
│   └── Program.cs (IoC setup)
│
└── DDD_ARCHITECTURE.md               (This documentation)
```

## 🚀 Quick Start: Adding a New Feature

### Step 1: Define the Domain Entity

Create in `Core.Domain/Entities/Product.cs`:

```csharp
public class Product : AggregateRoot<Guid>
{
    private Product(Guid id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public static Product Create(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required");
        
        var product = new Product(Guid.NewGuid(), name, price);
        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, name, price));
        return product;
    }
}

public class ProductCreatedEvent : DomainEvent
{
    public ProductCreatedEvent(Guid productId, string name, decimal price)
    {
        ProductId = productId;
        Name = name;
        Price = price;
    }

    public Guid ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }
}
```

### Step 2: Create Repository Interface

In `Core.Domain/Repositories/IProductRepository.cs`:

```csharp
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<List<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
}
```

### Step 3: Create Application DTOs

In `Core.Application/DTOs/ProductDTO.cs`:

```csharp
public record ProductDTO(
    Guid Id,
    string Name,
    decimal Price
);
```

### Step 4: Create Commands & Handlers

In `Core.Application/Features/Products/CreateProduct/`:

```csharp
public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = Product.Create(command.Name, command.Price);
        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
```

### Step 5: Implement Repository

In `Infrastructure/Persistence/Repositories/ProductRepository.cs`:

```csharp
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Product aggregate, CancellationToken cancellationToken)
        => await _context.Products.AddAsync(aggregate, cancellationToken);

    // ... implement other methods
}
```

### Step 6: Register in IoC

Update `Infrastructure/Extensions/ServiceCollectionExtensions.cs`:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

### Step 7: Use in Blazor Component

```csharp
@inject IMediator Mediator

<button @onclick="CreateProduct">Create Product</button>

@code {
    private async Task CreateProduct()
    {
        var command = new CreateProductCommand("Widget", 29.99m);
        var productId = await Mediator.SendAsync(command);
        Console.WriteLine($"Created product: {productId}");
    }
}
```

## 🔒 Dependency Rules (Enforced)

✅ **ALLOWED:**
```
Shared → (no one depends on shared)
Core.Domain → (Core.Domain has zero dependencies)
Core.Application → Core.Domain, MediatR
Infrastructure → Core.Domain, Core.Application
BlazorUI → Infrastructure, Core.Application, Shared, Core.Domain (indirectly through layers)
```

❌ **FORBIDDEN:**
```
Core.Domain → anything (must stay pure)
Core.Application → Infrastructure, BlazorUI
Infrastructure → BlazorUI
Circular dependencies
```

## 🎯 CQRS Pattern

The project uses **CQRS (Command Query Responsibility Segregation)** via MediatR:

### Commands (Write Operations)
- Modify state
- Return results or void
- Handled by `ICommandHandler<TCommand, TResponse>`

### Queries (Read Operations)
- No side effects
- Always return data
- Handled by `IQueryHandler<TQuery, TResponse>`

### Mediator
Central dispatcher that routes commands/queries to handlers:
```csharp
// Send command
var userId = await mediator.SendAsync(new CreateUserCommand("John", "john@example.com"));

// Execute query
var user = await mediator.QueryAsync(new GetUserQuery(userId));
```

## 📊 Aggregate Roots & Entities

### AggregateRoot
- Contains one or more entities
- Has a unique identity (root entity)
- Manages internal consistency
- Publishes domain events

### Entity
- Has a unique identity within context
- Mutable
- Lifecycle is meaningful

### ValueObject
- No identity
- Immutable
- Compared by value (equality based on components)

Example: `User` is an aggregate root, `Email` could be a value object.

## 🔄 Domain Events

Events represent facts about the domain:

```csharp
public class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Name { get; }
    // Can be published, logged, or trigger side effects
}
```

Events are collected in aggregates and published during `SaveChanges()`.

## 📦 Current Database

Using **Entity Framework Core In-Memory** for development. To switch to SQL Server:

1. Add package: `dotnet add Infrastructure package Microsoft.EntityFrameworkCore.SqlServer`
2. Uncomment `options.UseSqlServer(connectionString)` in Infrastructure extensions
3. Create migrations: `dotnet ef migrations add InitialCreate`
4. Update database: `dotnet ef database update`

## 🧪 Testing the Structure

The DDD structure is validated by:
- **Compile-time:** Project references (you can't reference restricted layers)
- **Runtime:** IoC container setup (interfaces segregation)
- **Architectural intent:** Clear separation of concerns

## 📚 Resources

- **Domain-Driven Design (Blue Book)** - Eric Evans
- **Implementing Domain-Driven Design** - Vaughn Vernon
- **Clean Architecture** - Robert C. Martin
- **MediatR Documentation** - https://github.com/jbogard/MediatR
- **Entity Framework Core** - https://learn.microsoft.com/en-us/ef/core/

## ⚙️ Next Steps

- [ ] Add more domain aggregates based on your business domain
- [ ] Implement event handlers for publishing domain events
- [ ] Add repository implementations for SQL Server
- [ ] Create Blazor pages that use mediator to dispatch commands/queries
- [ ] Add validation pipeline behaviors to MediatR
- [ ] Implement soft-delete or audit trails for entities
- [ ] Add logging and monitoring to handlers
- [ ] Write unit tests for domain entities
- [ ] Write integration tests for handlers
- [ ] Add API layer (optional, if needed)

## 🎓 Architecture Summary

| Aspect | Implementation |
|--------|-----------------|
| **Pattern** | Domain-Driven Design + CQRS |
| **Mediator** | MediatR 14.2.0 |
| **ORM** | Entity Framework Core 10.0.11 |
| **Database** | In-Memory (easily switched) |
| **Framework** | .NET 10 |
| **UI** | Blazor Interactive Server |
| **Layer Isolation** | Project reference constraints |

Your project is now ready for enterprise-level application development with clear business logic separation and maintainable architecture! 🚀
