using Core.Application.Abstractions;
using Core.Application.DTOs;
using Core.Domain.Repositories;

namespace Core.Application.Features.Products.GetProduct;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    
    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }
    
    public async Task<ProductDto?> HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);

        if (product is null) 
            return null;

        return new ProductDto(
            product.Id,
            product.Name,
            product.FileName,
            product.Price,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt,
            product.IsActive);
    }
}