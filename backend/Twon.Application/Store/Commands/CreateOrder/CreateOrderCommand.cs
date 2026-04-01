using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Store.Commands.CreateOrder;

public record CreateOrderCommand(string UserId, List<string> ProductIds)
    : IRequest<BaseResult<OrderDto>>;

public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalTHB { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];
    public PaymentInfoDto? Payment { get; set; }
    public CheckoutInfoDto? CheckoutInfo { get; set; }
}

public class OrderItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal PriceTHB { get; set; }
    public OrderProductDto? Product { get; set; }
}

public class OrderProductDto
{
    public string Title { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
}

public class PaymentInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal AmountTHB { get; set; }
    public DateTime? TransferredAt { get; set; }
    public string? Note { get; set; }
}

public class CheckoutInfoDto
{
    public string BankName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string QrImageUrl { get; set; } = string.Empty;
}
