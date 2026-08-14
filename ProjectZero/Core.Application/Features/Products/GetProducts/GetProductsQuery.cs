using Core.Application.Abstractions;
using Core.Application.DTOs;

namespace Core.Application.Features.Products.GetProducts;

public record GetProductsQuery() : IQuery<IEnumerable<ProductDto>>;