using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Products.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    /// <summary>
    /// Handles the deletion of a product by its identifier.
    /// </summary>
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Load the existing product to delete, or fail fast if it does not exist.
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{request.ProductId}' was not found.");

        // Remove the product from the repository.
        await _productRepository.DeleteAsync(product, cancellationToken);

        // Persist the deletion to the database.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}