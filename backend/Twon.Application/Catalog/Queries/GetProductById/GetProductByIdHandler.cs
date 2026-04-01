using MediatR;
using Twon.Application.Common;
using Twon.Application.Catalog.Queries.GetProducts;
using Twon.Application.Catalog.Managers;

namespace Twon.Application.Catalog.Queries.GetProductById;

public class GetProductByIdHandler(CatalogManager manager)
    : IRequestHandler<GetProductByIdQuery, BaseResult<ProductDto>>
{
    public async Task<BaseResult<ProductDto>> Handle(
        GetProductByIdQuery request, CancellationToken cancellationToken)
        => await manager.GetProductByIdAsync(request.Id);
}
