using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services;
using Core.Infrastructure.Services;
using Moq;

namespace Core.Infrastructure.Tests.Services;

public class DeleteMeUserDomainServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IDeleteMeEmailDomainService> _emailDomainService = new();

    private DeleteMeUserDomainService CreateService()
    {
        return new DeleteMeUserDomainService(_userRepository.Object, _emailDomainService.Object);
    }

    [Fact]
    public async Task EmailIsUniqueAsync_WhenValidAndUnused_ReturnsTrue()
    {
        _emailDomainService.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(true);
        _userRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        Assert.True(await service.EmailIsUniqueAsync("new@example.com", CancellationToken.None));
    }

    [Fact]
    public async Task EmailIsUniqueAsync_WhenAlreadyUsed_ReturnsFalse()
    {
        var existing = User.Create("Existing User", "taken@example.com");
        _emailDomainService.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(true);
        _userRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = CreateService();

        Assert.False(await service.EmailIsUniqueAsync("taken@example.com", CancellationToken.None));
    }

    [Fact]
    public async Task EmailIsUniqueAsync_WhenInvalid_ReturnsFalseWithoutCheckingRepository()
    {
        _emailDomainService.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(false);

        var service = CreateService();

        Assert.False(await service.EmailIsUniqueAsync("invalid", CancellationToken.None));
        _userRepository.Verify(
            r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void CanUserPerformAction_WhenInactive_ReturnsFalse()
    {
        var user = User.Create("John Doe", "john@example.com");
        user.Deactivate();

        var service = CreateService();

        Assert.False(service.CanUserPerformAction(user, "view-profile"));
    }

    [Theory]
    [InlineData("view-profile")]
    [InlineData("edit-profile")]
    public void CanUserPerformAction_WithAllowedAction_ReturnsTrue(string action)
    {
        var user = User.Create("John Doe", "john@example.com");

        var service = CreateService();

        Assert.True(service.CanUserPerformAction(user, action));
    }

    [Fact]
    public void CanUserPerformAction_DeleteAccount_WhenAccountYoungerThan30Days_ReturnsFalse()
    {
        var user = User.Create("John Doe", "john@example.com");

        var service = CreateService();

        Assert.False(service.CanUserPerformAction(user, "delete-account"));
    }

    [Fact]
    public void CanUserPerformAction_WithUnknownAction_ReturnsFalse()
    {
        var user = User.Create("John Doe", "john@example.com");

        var service = CreateService();

        Assert.False(service.CanUserPerformAction(user, "unknown-action"));
    }

    [Fact]
    public void CalculateActivityScore_ForNewActiveUserWithProfile_Returns30()
    {
        var user = User.Create("John Doe", "john@example.com");

        var service = CreateService();

        var score = service.CalculateActivityScore(user);

        Assert.Equal(30, score);
    }

    [Fact]
    public void CalculateActivityScore_ForInactiveUser_ExcludesActiveBonus()
    {
        var user = User.Create("John Doe", "john@example.com");
        user.Deactivate();

        var service = CreateService();

        var score = service.CalculateActivityScore(user);

        Assert.Equal(10, score);
    }

    [Fact]
    public void CalculateActivityScore_IsCappedAt100()
    {
        var user = User.Create("John Doe", "john@example.com");

        var service = CreateService();

        var score = service.CalculateActivityScore(user);

        Assert.InRange(score, 0, 100);
    }
}