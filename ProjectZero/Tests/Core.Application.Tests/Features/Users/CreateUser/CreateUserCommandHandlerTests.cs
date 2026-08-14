using Core.Application.Abstractions;
using Core.Application.Features.Users.CreateUser;
using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Moq;

namespace Core.Application.Tests.Features.Users.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateUserCommandHandler CreateHandler()
    {
        return new CreateUserCommandHandler(_userRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsUserIdAndPersists()
    {
        var handler = CreateHandler();
        var command = new CreateUserCommand("John Doe", "john@example.com");

        var userId = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, userId);
        _userRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenEmailAlreadyExists_ThrowsValidationException()
    {
        var existing = User.Create("Existing User", "john@example.com");
        _userRepository
            .Setup(r => r.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();
        var command = new CreateUserCommand("John Doe", "john@example.com");

        var exception =
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));

        Assert.True(exception.Errors.ContainsKey("Email"));
        _userRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_CreatesUserWithCommandData()
    {
        User? addedUser = null;
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => addedUser = user);

        var handler = CreateHandler();
        var command = new CreateUserCommand("Jane Doe", "jane@example.com");

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(addedUser);
        Assert.Equal("Jane Doe", addedUser!.Name);
        Assert.Equal("jane@example.com", addedUser.Email);
        Assert.True(addedUser.IsActive);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateUserCommandHandler(null!, _unitOfWork.Object));
    }

    [Fact]
    public void Constructor_WithNullUnitOfWork_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateUserCommandHandler(_userRepository.Object, null!));
    }
}