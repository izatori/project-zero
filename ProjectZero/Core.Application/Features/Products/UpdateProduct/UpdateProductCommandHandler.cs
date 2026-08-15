using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Products.UpdateProduct;

/// <summary>
/// Handler for UpdateProductCommand.
/// Orchestrates domain logic and persistence.
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the update of an existing product.
    /// </summary>
    public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Load the existing product to update, or fail fast if it does not exist.
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{command.ProductId}' was not found.");

        // Apply the updated values (business logic is encapsulated in the entity).
        product.Update(command.Name, command.FileName, command.Price, command.Description);

        // Persist the product.
        await _productRepository.UpdateAsync(product, cancellationToken);

        // Save changes (includes publishing domain events).
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}