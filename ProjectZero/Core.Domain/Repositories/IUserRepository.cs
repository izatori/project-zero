using Core.Domain.Abstractions;
using Core.Domain.Entities;

namespace Core.Domain.Repositories;

/// <summary>
/// Repository interface for User aggregate root.
/// Defined in the domain layer - implementations are in Infrastructure.
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<List<User>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}