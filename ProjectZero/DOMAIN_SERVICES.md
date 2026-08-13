# Domain Services in DDD Architecture

## Where Do Domain Services Go?

```
Core.Domain/
  └─ Services/                    ← INTERFACES ONLY (Business logic contracts)
     ├─ IUserDomainService.cs
     ├─ IEmailDomainService.cs
     └─ ...

Core.Infrastructure/
  └─ Services/                    ← IMPLEMENTATIONS (with infrastructure access)
     ├─ UserDomainService.cs
     ├─ EmailDomainService.cs
     └─ ...
```

## What Are Domain Services?

Domain services contain **business logic** that:
- ❌ Doesn't naturally belong to a single entity or aggregate root
- ❌ Operates across multiple aggregates
- ❌ Represents a business process or calculation
- ✅ Is state-less (no internal state)
- ✅ Only depends on domain objects
- ✅ Lives in Core.Domain (interfaces) and Core.Infrastructure (implementations)

## Examples of Domain Services

### ✅ Good Domain Service Use Cases

```csharp
// 1. Email uniqueness validation
IEmailDomainService.EmailIsUniqueAsync(email)

// 2. Business rule validation across aggregates
IUserDomainService.CanUserPerformAction(user, action)

// 3. Complex calculations
IUserDomainService.CalculateActivityScore(user)

// 4. Workflow coordination between aggregates
IOrderDomainService.CheckInventoryAndReserveStock(order)

// 5. Domain-specific algorithms
IPricingDomainService.CalculateDiscount(customer, product)
```

### ❌ What NOT to Put in Domain Services

```csharp
// ❌ Data access (use repositories instead)
await _dbContext.Users.ToListAsync();

// ❌ Sending emails (use application services/handlers)
await _emailService.SendAsync(user.Email);

// ❌ Logging/telemetry (use infrastructure services)
_logger.LogInformation("User created");

// ❌ HTTP calls (use infrastructure services)
var result = await _httpClient.GetAsync(url);
```

## Layer Breakdown

### Core.Domain/Services (INTERFACES ONLY)
```csharp
public interface IUserDomainService
{
    Task<bool> EmailIsUniqueAsync(string email);
    bool CanUserPerformAction(User user, string action);
    int CalculateActivityScore(User user);
}
```

**Characteristics:**
- ✅ No dependencies on infrastructure
- ✅ Only depends on domain entities, value objects, repositories
- ✅ Represents business rules/calculations
- ✅ Stateless

### Core.Infrastructure/Services (IMPLEMENTATIONS)
```csharp
public class UserDomainService : IUserDomainService
{
    private readonly IUserRepository _repository;
    private readonly IEmailDomainService _emailService;
    
    // Can depend on repositories and other domain services
    // Can also depend on infrastructure services for cross-cutting concerns
}
```

**Characteristics:**
- ✅ Implements domain service interfaces
- ✅ Can use repositories to check business rules
- ✅ Orchestrates domain logic with infrastructure
- ✅ Still focuses on domain business logic

### Core.Application (USE Domain Services)
```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserDomainService _userDomainService;
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Guid> HandleAsync(CreateUserCommand command, ...)
    {
        // Use domain service for business validation
        var isEmailUnique = await _userDomainService.EmailIsUniqueAsync(command.Email);
        if (!isEmailUnique)
            throw new ValidationException("Email already exists");
        
        // Create domain entity
        var user = User.Create(command.Name, command.Email);
        
        // Persist
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return user.Id;
    }
}
```

## Key Differences

| Aspect | Entity/Aggregate | Domain Service | Application Service | Infrastructure Service |
|--------|------------------|-----------------|---------------------|----------------------|
| **Location** | Core.Domain | Core.Domain + Core.Infrastructure | Core.Application | Core.Infrastructure |
| **Has State** | ✓ Yes | ✗ No | ✗ No | ✗ No |
| **Encapsulates Logic** | ✓ Yes (own data) | ✓ Yes (cross-aggregate) | ✓ Yes (orchestration) | ✓ Yes (external) |
| **Example** | User entity | EmailDomainService | CreateUserHandler | EmailSender |

## Registration in IoC

```csharp
// In Core.Infrastructure/Extensions/ServiceCollectionExtensions.cs
services.AddScoped<IUserDomainService, UserDomainService>();
services.AddScoped<IEmailDomainService, EmailDomainService>();
```

## When to Use Each Pattern

### ✅ Use Entity/AggregateRoot
- Logic about a single entity
- State changes to the entity
- Encapsulated within the aggregate
```csharp
user.UpdateInfo(name, email); // Inside User entity
```

### ✅ Use Domain Service
- Logic across multiple aggregates
- Business process/calculation
- No natural owner aggregate
```csharp
_userDomainService.CanUserPerformAction(user, action);
```

### ✅ Use Application Service (Handler)
- Orchestration of domain services
- Coordinating business operations
- Transaction management
```csharp
var command = new CreateUserCommand(...);
await Mediator.SendAsync(command); // Application service
```

### ✅ Use Infrastructure Service
- External system integration
- Logging, caching, email
- Database queries beyond repositories
```csharp
await _emailService.SendAsync(email);
```

## Best Practices

1. **Keep domain services stateless** - No instance variables
2. **Inject dependencies** - Repositories, other domain services
3. **Define interfaces in Core.Domain** - Implementations in Core.Infrastructure
4. **Use in handlers** - Application services orchestrate domain services
5. **Test easily** - Mock interfaces for unit tests
6. **Single Responsibility** - One domain service per business capability

## Example: Complete Flow

```
CreateUserCommand (user input)
  ↓
CreateUserCommandHandler (Core.Application)
  ↓ Uses
IUserDomainService.EmailIsUniqueAsync() (Core.Domain + Core.Infrastructure)
  ↓ Depends on
IUserRepository.GetByEmailAsync() (Core.Domain interface, Core.Infrastructure impl)
  ↓ Uses
User.Create() (Core.Domain entity)
  ↓ Calls
_repository.AddAsync() (Core.Infrastructure)
_unitOfWork.SaveChangesAsync() (Core.Infrastructure)
  ↓
Success ✓
```

## Summary

- **Core.Domain/Services/** → Business logic interfaces (stateless, high-level rules)
- **Core.Infrastructure/Services/** → Implementation (may use repositories)
- **Register in IoC** → Make available to handlers
- **Use in handlers** → Application services orchestrate domain services
- **Keep pure** → Domain services should focus on business logic only
