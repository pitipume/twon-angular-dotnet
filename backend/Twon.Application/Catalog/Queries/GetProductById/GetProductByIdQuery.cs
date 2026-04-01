using MediatR;
using Twon.Application.Common;
using Twon.Application.Catalog.Queries.GetProducts;

namespace Twon.Application.Catalog.Queries.GetProductById;

public record GetProductByIdQuery(string Id) : IRequest<BaseResult<ProductDto>>;
