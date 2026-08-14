using Core.Application.Features.Products.CreateProduct;
using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Moq;

namespace Core.Application.Tests.Features.Products.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateProductCommandHandler CreateHandler()
    {
        return new CreateProductCommandHandler(_productRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsProductIdAndPersists()
    {
        var handler = CreateHandler();
        var command = new CreateProductCommand("Widget", "widget.jpg", "A handy widget", 19.99);

        var productId = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, productId);
        _productRepository.Verify(
            r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CreatesProductWithCommandData()
    {
        Product? addedProduct = null;
        _productRepository
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) => addedProduct = product);

        var handler = CreateHandler();
        var command = new CreateProductCommand("Widget", "widget.jpg", "A handy widget", 19.99);

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(addedProduct);
        Assert.Equal("Widget", addedProduct!.Name);
        Assert.Equal("widget.jpg", addedProduct.FileName);
        Assert.Equal(19.99, addedProduct.Price);
        Assert.Equal("A handy widget", addedProduct.Description);
        Assert.True(addedProduct.IsActive);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateProductCommandHandler(null!, _unitOfWork.Object));
    }

    [Fact]
    public void Constructor_WithNullUnitOfWork_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateProductCommandHandler(_productRepository.Object, null!));
    }
}
