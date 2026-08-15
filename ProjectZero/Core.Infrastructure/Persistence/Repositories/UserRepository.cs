using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User aggregate root.
/// Handles persistence of User entities using Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Tracks a new user so it will be inserted when the unit of work is saved.
    /// </summary>
    public async Task AddAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// Marks an existing user as modified so its changes are persisted on save.
    /// </summary>
    public async Task UpdateAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Marks a user for deletion so it is removed on save.
    /// </summary>
    public async Task DeleteAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a user by its identifier, or null if it does not exist.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by email address, or null if it does not exist.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Retrieves all active users.
    /// </summary>
    public async Task<List<User>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }
}