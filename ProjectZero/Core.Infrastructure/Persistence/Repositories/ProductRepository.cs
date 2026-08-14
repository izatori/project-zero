using Core.Domain.Entities;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context ??  throw new ArgumentNullException(nameof(context));
    }
    
    public async Task AddAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(aggregate, cancellationToken);
    }

    public async Task UpdateAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(aggregate);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(aggregate);
        await Task.CompletedTask;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
         return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }
}