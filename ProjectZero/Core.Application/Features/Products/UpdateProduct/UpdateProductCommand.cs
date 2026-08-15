using MediatR;

namespace Core.Application.Features.Products.UpdateProduct;

/// <summary>
/// Command to update an existing product.
/// Commands represent write operations in CQRS.
/// </summary>
public record UpdateProductCommand(Guid ProductId, string Name, string FileName, string Description, double Price) : IRequest;