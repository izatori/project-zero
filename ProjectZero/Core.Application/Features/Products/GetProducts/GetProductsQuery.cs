using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.GetProducts;

/// <summary>
/// Query to get all active products.
/// Queries represent read operations in CQRS.
/// </summary>
/// <param name="Limit">Optional maximum number of products to retrieve. Null means no limit.</param>
public record GetProductsQuery(int? Limit = null) : IRequest<IEnumerable<ProductDto>>;