using Core.Domain.Abstractions;

namespace Core.Domain.Tests.Abstractions;

public class EntityTests
{
    private sealed class TestEntity : Entity<int>
    {
        public TestEntity(int id) : base(id)
        {
        }
    }

    [Fact]
    public void TwoEntities_WithSameId_AreEqual()
    {
        var a = new TestEntity(1);
        var b = new TestEntity(1);

        Assert.Equal(a, b);
        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TwoEntities_WithDifferentIds_AreNotEqual()
    {
        var a = new TestEntity(1);
        var b = new TestEntity(2);

        Assert.NotEqual(a, b);
        Assert.False(a.Equals(b));
        Assert.True(a != b);
        Assert.False(a == b);
    }

    [Fact]
    public void Entity_ComparedToNull_IsNotEqual()
    {
        var a = new TestEntity(1);

        Assert.False(a.Equals(null));
        Assert.False(a == null);
        Assert.True(a != null);
    }

    [Fact]
    public void Entity_WithSameId_DifferentReference_HasEqualHashCode()
    {
        var a = new TestEntity(42);
        var b = new TestEntity(42);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}