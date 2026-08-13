using Core.Domain.Entities;
using Core.Infrastructure.Persistence;
using Core.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Tests.Persistence.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"UserRepositoryTests-{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsUser()
    {
        var user = User.Create("John Doe", "john@example.com");

        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        var user = User.Create("John Doe", "john@example.com");
        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByEmailAsync("john@example.com", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByEmailAsync("nobody@example.com", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsOnlyActiveUsers()
    {
        var active = User.Create("Active User", "active@example.com");
        var inactive = User.Create("Inactive User", "inactive@example.com");
        inactive.Deactivate();

        await _repository.AddAsync(active, CancellationToken.None);
        await _repository.AddAsync(inactive, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(CancellationToken.None);

        var user = Assert.Single(result);
        Assert.Equal(active.Id, user.Id);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        var user = User.Create("John Doe", "john@example.com");
        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        user.UpdateInfo("Jane Doe", "jane@example.com");

        await _repository.UpdateAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.Equal("Jane Doe", result!.Name);
        Assert.Equal("jane@example.com", result.Email);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        var user = User.Create("John Doe", "john@example.com");
        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        await _repository.DeleteAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.Null(result);
    }
}