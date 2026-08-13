using Core.Domain.Abstractions;

namespace Core.Domain.Tests.Abstractions;

public class AggregateRootTests
{
    private sealed class TestDomainEvent : DomainEvent
    {
        public TestDomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }

    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id)
        {
        }

        public void Raise() => RaiseDomainEvent(new TestDomainEvent(Id));
    }

    [Fact]
    public void GetDomainEvents_ReturnsRaisedEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.Raise();

        var domainEvent = Assert.Single(aggregate.GetDomainEvents());

        var raisedEvent = Assert.IsType<TestDomainEvent>(domainEvent);
        Assert.Equal(aggregate.Id, raisedEvent.AggregateId);
    }

    [Fact]
    public void GetDomainEvents_WhenNothingRaised_ReturnsEmpty()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        Assert.Empty(aggregate.GetDomainEvents());
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllRaisedEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.Raise();
        aggregate.Raise();

        Assert.Equal(2, aggregate.GetDomainEvents().Count);

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.GetDomainEvents());
    }
}