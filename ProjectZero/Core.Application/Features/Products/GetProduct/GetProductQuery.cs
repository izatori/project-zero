using Core.Application.Abstractions;
using Core.Application.DTOs;

namespace Core.Application.Features.Products.GetProduct;

/// <summary>
/// Query to get a product by ID.
/// Queries represent read operations in CQRS.
/// </summary>
public record GetProductQuery(Guid ProductId) : IQuery<ProductDto>;