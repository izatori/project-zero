using Core.Domain.Entities;
using Core.Infrastructure.Persistence;
using Core.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Tests.Persistence.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"ProductRepositoryTests-{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsProduct()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");

        await _repository.AddAsync(product, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
        Assert.Equal("Widget", result.Name);
        Assert.Equal("widget.jpg", result.FileName);
        Assert.Equal(19.99, result.Price);
        Assert.Equal("A handy widget", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsOnlyActiveProducts()
    {
        var active = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        var inactive = Product.Create("Gadget", "gadget.jpg", 29.99, "A shiny gadget");
        inactive.Deactivate();

        await _repository.AddAsync(active, CancellationToken.None);
        await _repository.AddAsync(inactive, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(cancellationToken: CancellationToken.None);

        var product = Assert.Single(result);
        Assert.Equal(active.Id, product.Id);
    }

    [Fact]
    public async Task GetAllActiveAsync_WithLimit_ReturnsLimitedProducts()
    {
        var first = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        var second = Product.Create("Gadget", "gadget.jpg", 29.99, "A shiny gadget");
        var third = Product.Create("Doohickey", "doohickey.jpg", 9.99, "A small doohickey");

        await _repository.AddAsync(first, CancellationToken.None);
        await _repository.AddAsync(second, CancellationToken.None);
        await _repository.AddAsync(third, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(2, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        await _repository.AddAsync(product, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        product.Update("Super Widget", null, 24.99, "An upgraded widget");

        await _repository.UpdateAsync(product, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.Equal("Super Widget", result!.Name);
        Assert.Equal("widget.jpg", result.FileName);
        Assert.Equal(24.99, result.Price);
        Assert.Equal("An upgraded widget", result.Description);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        await _repository.AddAsync(product, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        await _repository.DeleteAsync(product, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.Null(result);
    }
}