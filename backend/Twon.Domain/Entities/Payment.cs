using Twon.Domain.Enums;

namespace Twon.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public decimal AmountTHB { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;
    public string? SlipImageKey { get; set; }
    public DateTime? TransferredAt { get; set; }
    public string? Note { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Order Order { get; set; } = null!;
}
