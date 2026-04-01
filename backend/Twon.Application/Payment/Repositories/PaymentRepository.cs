using Twon.Application.Payment.Queries.GetPendingOrders;
using Twon.Domain.Entities;

namespace Twon.Application.Payment.Repositories;

public interface IPaymentRepository
{
    Task<Order?> FindOrderAsync(string orderId);
    Task SubmitSlipAsync(string orderId, string slipKey, DateTime transferredAt, string? note);
    Task ApprovePaymentAsync(string orderId, string adminId);
    Task RejectPaymentAsync(string orderId, string adminId, string reason);
    Task GrantLibraryAccessAsync(string userId, string orderId, List<string> productIds);
    Task<List<PendingOrderDto>> FindPendingOrdersAsync();
}

public class PaymentRepository(IPaymentRepository inner)
{
    public Task<Order?> FindOrderAsync(string orderId) => inner.FindOrderAsync(orderId);
    public Task SubmitSlipAsync(string orderId, string slipKey, DateTime transferredAt, string? note)
        => inner.SubmitSlipAsync(orderId, slipKey, transferredAt, note);
    public Task ApprovePaymentAsync(string orderId, string adminId)
        => inner.ApprovePaymentAsync(orderId, adminId);
    public Task RejectPaymentAsync(string orderId, string adminId, string reason)
        => inner.RejectPaymentAsync(orderId, adminId, reason);
    public Task GrantLibraryAccessAsync(string userId, string orderId, List<string> productIds)
        => inner.GrantLibraryAccessAsync(userId, orderId, productIds);
    public Task<List<PendingOrderDto>> FindPendingOrdersAsync()
        => inner.FindPendingOrdersAsync();
}
