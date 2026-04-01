namespace Twon.Application.Catalog.Queries.GetProducts;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string MongoRefId { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal PriceTHB { get; set; }
    public bool IsPublished { get; set; }
    // Enriched from MongoDB
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Language { get; set; }
    public List<string>? Categories { get; set; }
    public List<string>? Tags { get; set; }
    public int? CardCount { get; set; }
    public int? TotalPages { get; set; }
}
