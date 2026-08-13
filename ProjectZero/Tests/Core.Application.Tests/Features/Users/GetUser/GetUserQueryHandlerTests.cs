using Core.Application.Features.Users.GetUser;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Moq;

namespace Core.Application.Tests.Features.Users.GetUser;

public class GetUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();

    [Fact]
    public async Task HandleAsync_WhenUserExists_ReturnsDto()
    {
        var user = User.Create("John Doe", "john@example.com");
        _userRepository
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserQueryHandler(_userRepository.Object);
        var query = new GetUserQuery(user.Id);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal(user.CreatedAt, result.CreatedAt);
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
        Assert.Equal(user.IsActive, result.IsActive);
    }

    [Fact]
    public async Task HandleAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var handler = new GetUserQueryHandler(_userRepository.Object);
        var query = new GetUserQuery(Guid.NewGuid());

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GetUserQueryHandler(null!));
    }
}