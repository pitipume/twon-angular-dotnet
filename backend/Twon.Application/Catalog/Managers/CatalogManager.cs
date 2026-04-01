using Twon.Application.Common;
using Twon.Application.Catalog.Queries.GetProducts;
using Twon.Application.Catalog.Services;

namespace Twon.Application.Catalog.Managers;

public class CatalogManager(CatalogService service)
{
    public async Task<BaseResult<List<ProductDto>>> GetProductsAsync(string? type)
    {
        var products = await service.GetPublishedProductsAsync(type);
        return BaseResult<List<ProductDto>>.Success(products);
    }

    public async Task<BaseResult<ProductDto>> GetProductByIdAsync(string id)
    {
        var product = await service.GetProductByIdAsync(id);
        if (product is null) return BaseResult<ProductDto>.NotFound("Product not found.");
        return BaseResult<ProductDto>.Success(product);
    }
}
