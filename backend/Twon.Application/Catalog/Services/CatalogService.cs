using Twon.Application.Catalog.Queries.GetProducts;
using Twon.Application.Catalog.Repositories;

namespace Twon.Application.Catalog.Services;

public class CatalogService(CatalogRepository repository)
{
    public Task<List<ProductDto>> GetPublishedProductsAsync(string? type)
        => repository.FindPublishedProductsAsync(type);

    public Task<ProductDto?> GetProductByIdAsync(string id)
        => repository.FindProductByIdAsync(id);
}
