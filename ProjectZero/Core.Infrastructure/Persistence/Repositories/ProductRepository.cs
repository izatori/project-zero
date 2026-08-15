using Core.Domain.Entities;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Product aggregate root.
/// Handles persistence of Product entities using Entity Framework Core.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context ??  throw new ArgumentNullException(nameof(context));
    }
    
    /// <summary>
    /// Tracks a new product so it will be inserted when the unit of work is saved.
    /// </summary>
    public async Task AddAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// Marks an existing product as modified so its changes are persisted on save.
    /// </summary>
    public async Task UpdateAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Marks a product for deletion so it is removed on save.
    /// </summary>
    public async Task DeleteAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a product by its identifier, or null if it does not exist.
    /// </summary>
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
         return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves all active products ordered by creation date (newest first), optionally limited.
    /// </summary>
    public async Task<List<Product>> GetAllActiveAsync(int? limit = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products.Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt);

        if (limit is > 0)
            query = query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }
}