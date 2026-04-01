using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Catalog.Queries.GetProducts;

public record GetProductsQuery(string? Type) : IRequest<BaseResult<List<ProductDto>>>;
