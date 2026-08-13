namespace Core.Domain.Entities;

using Abstractions;

/// <summary>
/// Example User aggregate root demonstrating DDD patterns.
/// </summary>
public class User : AggregateRoot<Guid>
{
    // Private constructor for EF and deserialization
    private User(Guid id, string name, string email) : base(id)
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Factory method to create a new User.
    /// Contains all business logic for user creation.
    /// </summary>
    public static User Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        // Simple email validation
        if (!email.Contains("@"))
            throw new ArgumentException("Email format is invalid", nameof(email));

        var user = new User(Guid.NewGuid(), name, email);

        // Raise domain event
        user.RaiseDomainEvent(new UserCreatedEvent(user.Id, name, email));

        return user;
    }

    /// <summary>
    /// Update user information.
    /// </summary>
    public void UpdateInfo(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Email format is invalid", nameof(email));

        Name = name;
        Email = email;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new UserUpdatedEvent(Id, name, email));
    }

    /// <summary>
    /// Deactivate the user.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new UserDeactivatedEvent(Id));
    }
}

/// <summary>
/// Domain event raised when a user is created.
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    public UserCreatedEvent(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }

    public Guid UserId { get; }
    public string Name { get; }
    public string Email { get; }
}

/// <summary>
/// Domain event raised when a user is updated.
/// </summary>
public class UserUpdatedEvent : DomainEvent
{
    public UserUpdatedEvent(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }

    public Guid UserId { get; }
    public string Name { get; }
    public string Email { get; }
}

/// <summary>
/// Domain event raised when a user is deactivated.
/// </summary>
public class UserDeactivatedEvent : DomainEvent
{
    public UserDeactivatedEvent(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}