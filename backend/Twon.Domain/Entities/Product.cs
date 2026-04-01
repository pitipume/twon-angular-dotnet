using Twon.Domain.Enums;

namespace Twon.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MongoRefId { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal PriceTHB { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<LibraryItem> LibraryItems { get; set; } = [];
}
