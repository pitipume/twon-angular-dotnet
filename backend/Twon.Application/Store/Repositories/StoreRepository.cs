using Twon.Application.Store.Commands.CreateOrder;

namespace Twon.Application.Store.Repositories;

public interface IStoreRepository
{
    Task<List<(string Id, decimal PriceTHB)>> FindPublishedProductsByIdsAsync(List<string> ids);
    Task<List<string>> FindAlreadyOwnedAsync(string userId, List<string> productIds);
    Task<OrderDto> CreateOrderAsync(string userId, List<(string ProductId, decimal PriceTHB)> items);
    Task<OrderDto?> FindOrderByIdAsync(string orderId);
}

public class StoreRepository(IStoreRepository inner)
{
    public Task<List<(string Id, decimal PriceTHB)>> FindPublishedProductsByIdsAsync(List<string> ids)
        => inner.FindPublishedProductsByIdsAsync(ids);
    public Task<List<string>> FindAlreadyOwnedAsync(string userId, List<string> productIds)
        => inner.FindAlreadyOwnedAsync(userId, productIds);
    public Task<OrderDto> CreateOrderAsync(string userId, List<(string ProductId, decimal PriceTHB)> items)
        => inner.CreateOrderAsync(userId, items);
    public Task<OrderDto?> FindOrderByIdAsync(string orderId)
        => inner.FindOrderByIdAsync(orderId);
}
