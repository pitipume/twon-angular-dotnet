using Microsoft.EntityFrameworkCore;
using Twon.Application.Payment.Queries.GetPendingOrders;
using Twon.Application.Payment.Repositories;
using Twon.Domain.Entities;
using Twon.Domain.Enums;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Payment;

public class PaymentRepositoryImpl(TwonDbContext db) : IPaymentRepository
{
    public Task<Order?> FindOrderAsync(string orderId)
    {
        if (!Guid.TryParse(orderId, out var guid)) return Task.FromResult<Order?>(null);
        return db.Orders.Include(o => o.Payment).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == guid);
    }

    public async Task SubmitSlipAsync(string orderId, string slipKey, DateTime transferredAt, string? note)
    {
        var guid = Guid.Parse(orderId);
        var order = await db.Orders.Include(o => o.Payment).FirstAsync(o => o.Id == guid);

        if (order.Payment == null)
        {
            order.Payment = new Payment
            {
                OrderId = guid,
                AmountTHB = order.TotalTHB,
                Status = PaymentStatus.WAITING_APPROVAL,
                SlipImageKey = slipKey,
                TransferredAt = transferredAt,
                Note = note
            };
        }
        else
        {
            order.Payment.SlipImageKey = slipKey;
            order.Payment.TransferredAt = transferredAt;
            order.Payment.Note = note;
            order.Payment.Status = PaymentStatus.WAITING_APPROVAL;
            order.Payment.UpdatedAt = DateTime.UtcNow;
        }

        order.Status = OrderStatus.WAITING_APPROVAL;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task ApprovePaymentAsync(string orderId, string adminId)
    {
        var guid = Guid.Parse(orderId);
        var order = await db.Orders.Include(o => o.Payment).FirstAsync(o => o.Id == guid);

        order.Status = OrderStatus.COMPLETED;
        order.UpdatedAt = DateTime.UtcNow;

        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.APPROVED;
            order.Payment.ApprovedBy = adminId;
            order.Payment.ApprovedAt = DateTime.UtcNow;
            order.Payment.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public async Task RejectPaymentAsync(string orderId, string adminId, string reason)
    {
        var guid = Guid.Parse(orderId);
        var order = await db.Orders.Include(o => o.Payment).FirstAsync(o => o.Id == guid);

        order.Status = OrderStatus.REJECTED;
        order.UpdatedAt = DateTime.UtcNow;

        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.REJECTED;
            order.Payment.RejectionReason = reason;
            order.Payment.ApprovedBy = adminId;
            order.Payment.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public async Task GrantLibraryAccessAsync(string userId, string orderId, List<string> productIds)
    {
        var userGuid = Guid.Parse(userId);
        var orderGuid = Guid.Parse(orderId);
        var productGuids = productIds.Select(Guid.Parse).ToList();

        var existing = await db.LibraryItems
            .Where(li => li.UserId == userGuid && productGuids.Contains(li.ProductId))
            .Select(li => li.ProductId).ToListAsync();

        var toAdd = productGuids.Except(existing).Select(productId => new LibraryItem
        {
            UserId = userGuid,
            ProductId = productId,
            OrderId = orderGuid
        });

        db.LibraryItems.AddRange(toAdd);
        await db.SaveChangesAsync();
    }

    public async Task<List<PendingOrderDto>> FindPendingOrdersAsync()
    {
        var orders = await db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .Where(o => o.Status == OrderStatus.PENDING || o.Status == OrderStatus.WAITING_APPROVAL)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(o => new PendingOrderDto
        {
            Id = o.Id.ToString(),
            Status = o.Status.ToString(),
            TotalTHB = o.TotalTHB,
            CreatedAt = o.CreatedAt,
            User = new PendingUserDto
            {
                Id = o.User.Id.ToString(),
                Email = o.User.Email,
                DisplayName = o.User.DisplayName
            },
            OrderItems = o.OrderItems.Select(oi => new PendingOrderItemDto
            {
                ProductId = oi.ProductId.ToString(),
                PriceTHB = oi.PriceTHB,
                Product = new PendingProductDto
                {
                    Title = oi.Product.Title,
                    ProductType = oi.Product.ProductType.ToString()
                }
            }).ToList(),
            Payment = o.Payment == null ? null : new PendingPaymentDto
            {
                Status = o.Payment.Status.ToString(),
                AmountTHB = o.Payment.AmountTHB,
                TransferredAt = o.Payment.TransferredAt,
                Note = o.Payment.Note,
                SlipUrl = o.Payment.SlipImageKey  // caller resolves to signed URL
            }
        }).ToList();
    }
}
