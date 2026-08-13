using Core.Infrastructure.Services;

namespace Core.Infrastructure.Tests.Services;

public class DeleteMeEmailDomainServiceTests
{
    private readonly DeleteMeEmailDomainService _service = new();

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("first.last@sub.domain.co")]
    [InlineData("USER+tag@example.com")]
    public void IsValidEmail_WithValidEmail_ReturnsTrue(string email)
    {
        Assert.True(_service.IsValidEmail(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("plainaddress")]
    [InlineData("user@")]
    [InlineData("@domain.com")]
    [InlineData("user@domain")]
    [InlineData("user name@example.com")]
    public void IsValidEmail_WithInvalidEmail_ReturnsFalse(string email)
    {
        Assert.False(_service.IsValidEmail(email));
    }

    [Fact]
    public void IsValidEmail_WithNull_ReturnsFalse()
    {
        Assert.False(_service.IsValidEmail(null!));
    }

    [Fact]
    public void IsValidEmail_WhenLongerThan254Chars_ReturnsFalse()
    {
        var email = $"{new string('a', 250)}@example.com";

        Assert.False(_service.IsValidEmail(email));
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user@gmail.com")]
    public async Task IsEmailDomainAllowedAsync_WithAllowedDomain_ReturnsTrue(string email)
    {
        Assert.True(await _service.IsEmailDomainAllowedAsync(email, CancellationToken.None));
    }

    [Theory]
    [InlineData("user@tempmail.com")]
    [InlineData("user@throwaway.email")]
    [InlineData("user@guerrillamail.com")]
    public async Task IsEmailDomainAllowedAsync_WithBlacklistedDomain_ReturnsFalse(string email)
    {
        Assert.False(await _service.IsEmailDomainAllowedAsync(email, CancellationToken.None));
    }

    [Fact]
    public async Task IsEmailDomainAllowedAsync_WithInvalidEmail_ReturnsFalse()
    {
        Assert.False(await _service.IsEmailDomainAllowedAsync("invalid", CancellationToken.None));
    }
}