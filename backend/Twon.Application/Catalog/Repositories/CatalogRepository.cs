using Twon.Application.Catalog.Queries.GetProducts;

namespace Twon.Application.Catalog.Repositories;

public interface ICatalogRepository
{
    Task<List<ProductDto>> FindPublishedProductsAsync(string? type);
    Task<ProductDto?> FindProductByIdAsync(string id);
}

public class CatalogRepository(ICatalogRepository inner)
{
    public Task<List<ProductDto>> FindPublishedProductsAsync(string? type)
        => inner.FindPublishedProductsAsync(type);
    public Task<ProductDto?> FindProductByIdAsync(string id)
        => inner.FindProductByIdAsync(id);
}
