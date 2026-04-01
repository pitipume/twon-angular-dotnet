using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Twon.Infrastructure.Persistence;

// Document models
public class EbookDocument
{
    [BsonId][BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string PostgresProductId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Language { get; set; } = null!;
    public List<string> Categories { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public string FileKey { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public int TotalPages { get; set; }
    public int PreviewPages { get; set; }
    public bool IsPublished { get; set; }
}

public class TarotDeckDocument
{
    [BsonId][BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string PostgresProductId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public string? BackImageKey { get; set; }
    public List<TarotCardDocument> Cards { get; set; } = [];
    public bool IsPublished { get; set; }
}

public class TarotCardDocument
{
    public int CardNumber { get; set; }
    public string Name { get; set; } = null!;
    public string ImageKey { get; set; } = null!;
    public string UprightMeaning { get; set; } = string.Empty;
    public string ReversedMeaning { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = [];
}

public class ReadingProgressDocument
{
    [BsonId][BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public int CurrentPage { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Context
public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        _db = client.GetDatabase(config["MongoDB:DatabaseName"] ?? "twon");
    }

    public IMongoCollection<EbookDocument> EbookDocuments
        => _db.GetCollection<EbookDocument>("ebooks");
    public IMongoCollection<TarotDeckDocument> TarotDeckDocuments
        => _db.GetCollection<TarotDeckDocument>("tarot_decks");
    public IMongoCollection<ReadingProgressDocument> ReadingProgressDocuments
        => _db.GetCollection<ReadingProgressDocument>("reading_progress");
}
