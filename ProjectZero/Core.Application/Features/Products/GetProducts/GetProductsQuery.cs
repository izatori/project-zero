using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.GetProducts;

public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;