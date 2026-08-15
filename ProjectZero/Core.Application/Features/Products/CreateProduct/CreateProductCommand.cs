using MediatR;

namespace Core.Application.Features.Products.CreateProduct;

/// <summary>
/// Command to create a new product.
/// Commands represent write operations in CQRS.
/// </summary>
public record CreateProductCommand(
    string Name, 
    string FileName, 
    string Description, 
    double Price) : IRequest<Guid>;