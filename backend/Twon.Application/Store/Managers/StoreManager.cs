using Twon.Application.Common;
using Twon.Application.Common.Interfaces;
using Twon.Application.Store.Commands.CreateOrder;
using Twon.Application.Store.Services;

namespace Twon.Application.Store.Managers;

public class StoreManager(StoreService service, IStorageService storage)
{
    public async Task<BaseResult<OrderDto>> CreateOrderAsync(string userId, List<string> productIds)
    {
        var products = await service.GetPublishedProductsAsync(productIds);
        if (products.Count != productIds.Count)
            return BaseResult<OrderDto>.NotFound("One or more products not found.");

        var alreadyOwned = await service.GetAlreadyOwnedAsync(userId, productIds);
        if (alreadyOwned.Count > 0)
            return BaseResult<OrderDto>.Conflict("You already own one or more of these products.");

        var items = products.Select(p => (p.Id, p.PriceTHB)).ToList();
        var order = await service.CreateOrderAsync(userId, items);
        return BaseResult<OrderDto>.Success(order);
    }

    public async Task<BaseResult<OrderDto>> GetOrderAsync(string userId, string orderId)
    {
        var order = await service.GetOrderAsync(orderId);
        if (order is null) return BaseResult<OrderDto>.NotFound("Order not found.");
        if (order.UserId != userId) return BaseResult<OrderDto>.Unauthorized("Access denied.");

        if (order.CheckoutInfo is not null && !string.IsNullOrEmpty(order.CheckoutInfo.QrImageUrl))
        {
            var qrUrl = await storage.GetSignedReadUrlAsync(order.CheckoutInfo.QrImageUrl, 30 * 60);
            order.CheckoutInfo.QrImageUrl = qrUrl;
        }

        return BaseResult<OrderDto>.Success(order);
    }
}
