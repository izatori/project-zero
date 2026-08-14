using Core.Application.Features.Products.GetProducts;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Moq;

namespace Core.Application.Tests.Features.Products.GetProducts;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();

    [Fact]
    public async Task Handle_ReturnsMappedDtos()
    {
        var products = new List<Product>
        {
            Product.Create("Widget", "widget.jpg", 19.99, "A handy widget"),
            Product.Create("Gadget", "gadget.jpg", 29.99, "A shiny gadget")
        };
        _productRepository
            .Setup(r => r.GetAllActiveAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(_productRepository.Object);
        var query = new GetProductsQuery();

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Widget", result[0].Name);
        Assert.Equal("widget.jpg", result[0].FileName);
        Assert.Equal(19.99, result[0].Price);
        Assert.Equal("A handy widget", result[0].Description);
    }

    [Fact]
    public async Task Handle_ForwardsLimitToRepository()
    {
        _productRepository
            .Setup(r => r.GetAllActiveAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var handler = new GetProductsQueryHandler(_productRepository.Object);
        var query = new GetProductsQuery(Limit: 3);

        await handler.Handle(query, CancellationToken.None);

        _productRepository.Verify(
            r => r.GetAllActiveAsync(3, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GetProductsQueryHandler(null!));
    }
}