namespace Core.Domain.Abstractions;

/// <summary>
/// Base class for domain events.
/// Domain events represent something that has happened in the domain.
/// They capture the facts about the domain and can be used for event sourcing,
/// notifications, and maintaining eventual consistency across bounded contexts.
/// </summary>
public abstract class DomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    public Guid Id { get; }

    public DateTime OccurredOnUtc { get; }
}