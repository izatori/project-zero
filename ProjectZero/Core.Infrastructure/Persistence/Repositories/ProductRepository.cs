using Core.Domain.Entities;
using Core.Domain.Repositories;

namespace Core.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ProductRepository(ApplicationDbContext context)
    {
        _dbContext = context ??  throw new ArgumentNullException(nameof(context));
    }
    
    public Task AddAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}