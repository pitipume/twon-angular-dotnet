using Twon.Application.Payment.Queries.GetPendingOrders;
using Twon.Application.Payment.Repositories;
using Twon.Domain.Entities;

namespace Twon.Application.Payment.Services;

public class PaymentService(PaymentRepository repository)
{
    public Task<Order?> FindOrderAsync(string orderId) => repository.FindOrderAsync(orderId);
    public Task SubmitSlipAsync(string orderId, string slipKey, DateTime transferredAt, string? note)
        => repository.SubmitSlipAsync(orderId, slipKey, transferredAt, note);
    public Task ApprovePaymentAsync(string orderId, string adminId)
        => repository.ApprovePaymentAsync(orderId, adminId);
    public Task RejectPaymentAsync(string orderId, string adminId, string reason)
        => repository.RejectPaymentAsync(orderId, adminId, reason);
    public Task GrantLibraryAccessAsync(string userId, string orderId, List<string> productIds)
        => repository.GrantLibraryAccessAsync(userId, orderId, productIds);
    public Task<List<PendingOrderDto>> GetPendingOrdersAsync()
        => repository.FindPendingOrdersAsync();
}
