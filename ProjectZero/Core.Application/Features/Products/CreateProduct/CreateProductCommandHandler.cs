using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Products.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand.
/// Orchestrates domain logic and persistence.
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the creation of a new product.
    /// </summary>
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Create the domain entity (business logic is encapsulated in the entity)
        var product = Product.Create(command.Name, command.FileName, command.Price, command.Description);

        // Persist the product
        await _productRepository.AddAsync(product, cancellationToken);

        // Save changes (includes publishing domain events)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}