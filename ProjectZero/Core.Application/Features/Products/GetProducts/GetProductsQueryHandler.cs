using Core.Application.DTOs;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Products.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllActiveAsync(cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.FileName,
            p.Price,
            p.Description,
            p.CreatedAt,
            p.UpdatedAt,
            p.IsActive));
    }
}
