using MediatR;
using Twon.Application.Common;
using Twon.Application.Catalog.Managers;

namespace Twon.Application.Catalog.Queries.GetProducts;

public class GetProductsHandler(CatalogManager manager)
    : IRequestHandler<GetProductsQuery, BaseResult<List<ProductDto>>>
{
    public async Task<BaseResult<List<ProductDto>>> Handle(
        GetProductsQuery request, CancellationToken cancellationToken)
        => await manager.GetProductsAsync(request.Type);
}
