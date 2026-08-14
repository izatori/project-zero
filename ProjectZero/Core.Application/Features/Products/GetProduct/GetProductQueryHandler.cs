using Core.Application.DTOs;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Products.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    
    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }
    
    public async Task<ProductDto?> Handle(GetProductQuery query, CancellationToken cancellationToken = default)
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