using Microsoft.AspNetCore.Http;
using Twon.Application.Common;
using Twon.Application.Common.Interfaces;
using Twon.Application.Payment.Queries.GetPendingOrders;
using Twon.Application.Payment.Services;
using Twon.Domain.Enums;

namespace Twon.Application.Payment.Managers;

public class PaymentManager(PaymentService service, IStorageService storage)
{
    public async Task<BaseResult<object>> SubmitSlipAsync(
        string userId, string orderId, IFormFile file, DateTime transferredAt, string? note)
    {
        var order = await service.FindOrderAsync(orderId);
        if (order is null) return BaseResult<object>.NotFound("Order not found.");
        if (order.UserId.ToString() != userId) return BaseResult<object>.Unauthorized("Access denied.");
        if (order.Status != OrderStatus.PENDING)
            return BaseResult<object>.Conflict("Order is not in PENDING status.");

        var key = $"slips/{orderId}/slip{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();
        await storage.UploadAsync(key, stream, file.ContentType);

        await service.SubmitSlipAsync(orderId, key, transferredAt, note);
        return BaseResult<object>.Success(null!, "Slip submitted successfully.");
    }

    public async Task<BaseResult<object>> ApprovePaymentAsync(string adminId, string orderId)
    {
        var order = await service.FindOrderAsync(orderId);
        if (order is null) return BaseResult<object>.NotFound("Order not found.");
        if (order.Status != OrderStatus.WAITING_APPROVAL)
            return BaseResult<object>.Conflict("Order is not waiting for approval.");

        await service.ApprovePaymentAsync(orderId, adminId);
        var productIds = order.OrderItems.Select(i => i.ProductId.ToString()).ToList();
        await service.GrantLibraryAccessAsync(order.UserId.ToString(), orderId, productIds);

        return BaseResult<object>.Success(null!, "Payment approved.");
    }

    public async Task<BaseResult<object>> RejectPaymentAsync(string adminId, string orderId, string reason)
    {
        var order = await service.FindOrderAsync(orderId);
        if (order is null) return BaseResult<object>.NotFound("Order not found.");
        if (order.Status != OrderStatus.WAITING_APPROVAL)
            return BaseResult<object>.Conflict("Order is not waiting for approval.");

        await service.RejectPaymentAsync(orderId, adminId, reason);
        return BaseResult<object>.Success(null!, "Payment rejected.");
    }

    public async Task<BaseResult<List<PendingOrderDto>>> GetPendingOrdersAsync()
    {
        var orders = await service.GetPendingOrdersAsync();
        var result = await Task.WhenAll(orders.Select(async o =>
        {
            if (o.Payment?.SlipUrl is not null)
                o.Payment.SlipUrl = await storage.GetSignedReadUrlAsync(o.Payment.SlipUrl, 3600);
            return o;
        }));
        return BaseResult<List<PendingOrderDto>>.Success([.. result]);
    }
}
