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

    public async Task AddAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(aggregate, cancellationToken);
    }

    public async Task UpdateAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(aggregate);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(aggregate);
        await Task.CompletedTask;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<List<User>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }
}