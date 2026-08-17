using Core.Domain.Entities;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"UnitOfWorkTests-{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsAddedEntities()
    {
        _context.Products.Add(Product.Create("Widget", "widget.jpg", 9.99, "A widget"));

        var affectedRows = await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        Assert.Equal(1, affectedRows);
        Assert.Equal(1, await _context.Products.CountAsync(CancellationToken.None));
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
    {
        var affectedRows = await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        Assert.Equal(0, affectedRows);
    }
}