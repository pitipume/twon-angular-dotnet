using Microsoft.EntityFrameworkCore;
using Twon.Application.Store.Commands.CreateOrder;
using Twon.Application.Store.Repositories;
using Twon.Domain.Entities;
using Twon.Domain.Enums;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Store;

public class StoreRepositoryImpl(TwonDbContext db) : IStoreRepository
{
    public async Task<List<(string Id, decimal PriceTHB)>> FindPublishedProductsByIdsAsync(List<string> ids)
    {
        var guids = ids.Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
                       .Where(g => g != Guid.Empty).ToList();

        var rows = await db.Products
            .Where(p => guids.Contains(p.Id) && p.IsPublished)
            .Select(p => new { p.Id, p.PriceTHB })
            .ToListAsync();
        return rows.Select(p => (p.Id.ToString(), p.PriceTHB)).ToList();
    }

    public async Task<List<string>> FindAlreadyOwnedAsync(string userId, List<string> productIds)
    {
        if (!Guid.TryParse(userId, out var userGuid)) return [];
        var productGuids = productIds
            .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty).ToList();

        return await db.LibraryItems
            .Where(li => li.UserId == userGuid && productGuids.Contains(li.ProductId))
            .Select(li => li.ProductId.ToString())
            .ToListAsync();
    }

    public async Task<OrderDto> CreateOrderAsync(string userId, List<(string ProductId, decimal PriceTHB)> items)
    {
        var userGuid = Guid.Parse(userId);
        var total = items.Sum(i => i.PriceTHB);

        var order = new Order
        {
            UserId = userGuid,
            TotalTHB = total,
            Status = OrderStatus.PENDING,
            OrderItems = items.Select(i => new OrderItem
            {
                ProductId = Guid.Parse(i.ProductId),
                PriceTHB = i.PriceTHB
            }).ToList()
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return await MapToOrderDtoAsync(order.Id);
    }

    public Task<OrderDto?> FindOrderByIdAsync(string orderId)
    {
        if (!Guid.TryParse(orderId, out var guid)) return Task.FromResult<OrderDto?>(null);
        return MapToOrderDtoNullableAsync(guid);
    }

    private async Task<OrderDto> MapToOrderDtoAsync(Guid orderId)
    {
        var order = await db.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .FirstAsync(o => o.Id == orderId);

        var config = await db.PaymentConfigs.FirstOrDefaultAsync();

        return new OrderDto
        {
            Id = order.Id.ToString(),
            UserId = order.UserId.ToString(),
            Status = order.Status.ToString(),
            TotalTHB = order.TotalTHB,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id.ToString(),
                ProductId = oi.ProductId.ToString(),
                PriceTHB = oi.PriceTHB,
                Product = new OrderProductDto
                {
                    Title = oi.Product.Title,
                    ProductType = oi.Product.ProductType.ToString()
                }
            }).ToList(),
            Payment = order.Payment == null ? null : new PaymentInfoDto
            {
                Id = order.Payment.Id.ToString(),
                Status = order.Payment.Status.ToString(),
                AmountTHB = order.Payment.AmountTHB,
                TransferredAt = order.Payment.TransferredAt,
                Note = order.Payment.Note
            },
            CheckoutInfo = config == null ? null : new CheckoutInfoDto
            {
                BankName = config.BankName,
                AccountName = config.AccountName,
                AccountNumber = config.AccountNumber,
                QrImageUrl = config.QrImageKey  // caller resolves to signed URL
            }
        };
    }

    private async Task<OrderDto?> MapToOrderDtoNullableAsync(Guid orderId)
    {
        var exists = await db.Orders.AnyAsync(o => o.Id == orderId);
        if (!exists) return null;
        return await MapToOrderDtoAsync(orderId);
    }
}
