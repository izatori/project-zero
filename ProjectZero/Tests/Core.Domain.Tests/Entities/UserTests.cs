using Core.Domain.Entities;

namespace Core.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ReturnsUser()
    {
        var user = User.Create("John Doe", "john@example.com");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("john@example.com", user.Email);
        Assert.True(user.IsActive);
        Assert.Null(user.UpdatedAt);
        Assert.InRange(user.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() => User.Create(name!, "john@example.com"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ThrowsArgumentException(string? email)
    {
        Assert.Throws<ArgumentException>(() => User.Create("John Doe", email!));
    }

    [Fact]
    public void Create_WithInvalidEmail_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => User.Create("John Doe", "not-an-email"));
    }

    [Fact]
    public void Create_RaisesUserCreatedEvent()
    {
        var user = User.Create("John Doe", "john@example.com");

        var domainEvent = Assert.Single(user.GetDomainEvents());
        var created = Assert.IsType<UserCreatedEvent>(domainEvent);

        Assert.Equal(user.Id, created.UserId);
        Assert.Equal("John Doe", created.Name);
        Assert.Equal("john@example.com", created.Email);
    }

    [Fact]
    public void UpdateInfo_WithValidData_UpdatesUserAndRaisesEvent()
    {
        var user = User.Create("John Doe", "john@example.com");
        user.ClearDomainEvents();

        user.UpdateInfo("Jane Doe", "jane@example.com");

        Assert.Equal("Jane Doe", user.Name);
        Assert.Equal("jane@example.com", user.Email);
        Assert.NotNull(user.UpdatedAt);

        var domainEvent = Assert.Single(user.GetDomainEvents());
        Assert.IsType<UserUpdatedEvent>(domainEvent);
    }

    [Fact]
    public void UpdateInfo_WithInvalidEmail_ThrowsArgumentException()
    {
        var user = User.Create("John Doe", "john@example.com");

        Assert.Throws<ArgumentException>(() => user.UpdateInfo("Jane Doe", "invalid"));
    }

    [Fact]
    public void UpdateInfo_WithEmptyName_ThrowsArgumentException()
    {
        var user = User.Create("John Doe", "john@example.com");

        Assert.Throws<ArgumentException>(() => user.UpdateInfo(string.Empty, "jane@example.com"));
    }

    [Fact]
    public void Deactivate_WhenActive_SetsInactiveAndRaisesEvent()
    {
        var user = User.Create("John Doe", "john@example.com");
        user.ClearDomainEvents();

        user.Deactivate();

        Assert.False(user.IsActive);
        Assert.NotNull(user.UpdatedAt);

        var domainEvent = Assert.Single(user.GetDomainEvents());
        Assert.IsType<UserDeactivatedEvent>(domainEvent);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_DoesNotRaiseEvent()
    {
        var user = User.Create("John Doe", "john@example.com");
        user.Deactivate();
        user.ClearDomainEvents();

        user.Deactivate();

        Assert.Empty(user.GetDomainEvents());
    }

    [Fact]
    public void TwoUsers_WithDifferentIds_AreNotEqual()
    {
        var user = User.Create("John Doe", "john@example.com");
        var other = User.Create("Jane Doe", "jane@example.com");

        Assert.NotEqual(user.Id, other.Id);
        Assert.NotEqual(user, other);
    }
}