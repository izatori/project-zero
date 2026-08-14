using Core.Domain.Abstractions;
using Core.Domain.Entities;

namespace Core.Domain.Repositories;

/// <summary>
/// Repository interface for Product aggregate root.
/// Defined in the domain layer - implementations are in Infrastructure.
/// </summary>
public interface IProductRepository  : IRepository<Product, Guid>
{
    Task<List<Product>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}