using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Payment.Queries.GetPendingOrders;

public record GetPendingOrdersQuery() : IRequest<BaseResult<List<PendingOrderDto>>>;

public class PendingOrderDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalTHB { get; set; }
    public DateTime CreatedAt { get; set; }
    public PendingUserDto User { get; set; } = null!;
    public List<PendingOrderItemDto> OrderItems { get; set; } = [];
    public PendingPaymentDto? Payment { get; set; }
}

public class PendingUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class PendingOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal PriceTHB { get; set; }
    public PendingProductDto? Product { get; set; }
}

public class PendingProductDto
{
    public string Title { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
}

public class PendingPaymentDto
{
    public string Status { get; set; } = string.Empty;
    public decimal AmountTHB { get; set; }
    public DateTime? TransferredAt { get; set; }
    public string? Note { get; set; }
    public string? SlipUrl { get; set; }
}
