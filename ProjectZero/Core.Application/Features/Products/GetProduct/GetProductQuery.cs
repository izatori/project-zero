using Core.Application.Abstractions;

namespace Core.Application.Features.Products.GetProduct;

/// <summary>
/// Query to get a product by ID.
/// Queries represent read operations in CQRS.
/// </summary>
public record GetProductQuery(Guid ProductId) : IQuery<Guid>;