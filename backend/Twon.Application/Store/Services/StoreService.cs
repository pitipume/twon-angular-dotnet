using Twon.Application.Store.Commands.CreateOrder;
using Twon.Application.Store.Repositories;

namespace Twon.Application.Store.Services;

public class StoreService(StoreRepository repository)
{
    public Task<List<(string Id, decimal PriceTHB)>> GetPublishedProductsAsync(List<string> ids)
        => repository.FindPublishedProductsByIdsAsync(ids);

    public Task<List<string>> GetAlreadyOwnedAsync(string userId, List<string> productIds)
        => repository.FindAlreadyOwnedAsync(userId, productIds);

    public Task<OrderDto> CreateOrderAsync(string userId, List<(string ProductId, decimal PriceTHB)> items)
        => repository.CreateOrderAsync(userId, items);

    public Task<OrderDto?> GetOrderAsync(string orderId)
        => repository.FindOrderByIdAsync(orderId);
}
