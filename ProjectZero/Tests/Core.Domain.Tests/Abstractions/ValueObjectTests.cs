using Core.Domain.Abstractions;

namespace Core.Domain.Tests.Abstractions;

public class ValueObjectTests
{
    private sealed class Money : ValueObject
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; }

        public string Currency { get; }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    [Fact]
    public void ValueObjects_WithSameComponents_AreEqual()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "USD");

        Assert.Equal(a, b);
        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ValueObjects_WithDifferentAmount_AreNotEqual()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "USD");

        Assert.NotEqual(a, b);
        Assert.True(a != b);
        Assert.False(a == b);
    }

    [Fact]
    public void ValueObjects_WithDifferentCurrency_AreNotEqual()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "EUR");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ValueObject_ComparedToNull_IsNotEqual()
    {
        var a = new Money(100m, "USD");

        Assert.False(a.Equals(null));
        Assert.False(a == null);
        Assert.True(a != null);
    }

    [Fact]
    public void ValueObject_ComparedToDifferentType_IsNotEqual()
    {
        var a = new Money(100m, "USD");

        Assert.False(a.Equals(new object()));
    }
}