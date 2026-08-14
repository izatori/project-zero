using Core.Application.Abstractions;

namespace Core.Application.Features.Products.CreateProduct;

public record CreateProductCommand(string Name, string fileName, string Description, double Price) : ICommand<Guid>;