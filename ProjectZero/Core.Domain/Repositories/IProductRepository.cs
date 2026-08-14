using Core.Domain.Abstractions;
using Core.Domain.Entities;

namespace Core.Domain.Repositories;

public interface IProductRepository  : IRepository<Product, Guid>
{
    Task<List<Product>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}