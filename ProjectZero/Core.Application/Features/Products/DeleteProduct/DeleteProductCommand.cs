using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : IRequest;