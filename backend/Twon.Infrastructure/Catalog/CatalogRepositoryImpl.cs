using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Twon.Application.Catalog.Queries.GetProducts;
using Twon.Application.Catalog.Repositories;
using Twon.Domain.Enums;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Catalog;

public class CatalogRepositoryImpl(TwonDbContext db, MongoDbContext mongo) : ICatalogRepository
{
    public async Task<List<ProductDto>> FindPublishedProductsAsync(string? type)
    {
        var query = db.Products.Where(p => p.IsPublished);
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<ProductType>(type, true, out var pt))
            query = query.Where(p => p.ProductType == pt);

        var products = await query.ToListAsync();
        var result = new List<ProductDto>();

        foreach (var p in products)
        {
            var dto = new ProductDto
            {
                Id = p.Id.ToString(),
                MongoRefId = p.MongoRefId,
                ProductType = p.ProductType.ToString(),
                Title = p.Title,
                PriceTHB = p.PriceTHB,
                IsPublished = p.IsPublished,
            };

            if (p.ProductType == ProductType.EBOOK)
            {
                var ebook = await mongo.EbookDocuments
                    .Find(e => e.PostgresProductId == p.Id.ToString()).FirstOrDefaultAsync();
                if (ebook != null)
                {
                    dto.Author = ebook.Author;
                    dto.Description = ebook.Description;
                    dto.CoverImageUrl = ebook.CoverImageUrl;
                    dto.Language = ebook.Language;
                    dto.Categories = ebook.Categories;
                    dto.Tags = ebook.Tags;
                    dto.TotalPages = ebook.TotalPages;
                }
            }
            else if (p.ProductType == ProductType.TAROT_DECK)
            {
                var deck = await mongo.TarotDeckDocuments
                    .Find(d => d.PostgresProductId == p.Id.ToString()).FirstOrDefaultAsync();
                if (deck != null)
                {
                    dto.Description = deck.Description;
                    dto.CoverImageUrl = deck.CoverImageUrl;
                    dto.CardCount = deck.Cards.Count;
                }
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<ProductDto?> FindProductByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        var p = await db.Products.FirstOrDefaultAsync(x => x.Id == guid);
        if (p == null) return null;

        var dto = new ProductDto
        {
            Id = p.Id.ToString(),
            MongoRefId = p.MongoRefId,
            ProductType = p.ProductType.ToString(),
            Title = p.Title,
            PriceTHB = p.PriceTHB,
            IsPublished = p.IsPublished,
        };

        if (p.ProductType == ProductType.EBOOK)
        {
            var ebook = await mongo.EbookDocuments
                .Find(e => e.PostgresProductId == id).FirstOrDefaultAsync();
            if (ebook != null)
            {
                dto.Author = ebook.Author;
                dto.Description = ebook.Description;
                dto.CoverImageUrl = ebook.CoverImageUrl;
                dto.Language = ebook.Language;
                dto.Categories = ebook.Categories;
                dto.Tags = ebook.Tags;
                dto.TotalPages = ebook.TotalPages;
            }
        }
        else if (p.ProductType == ProductType.TAROT_DECK)
        {
            var deck = await mongo.TarotDeckDocuments
                .Find(d => d.PostgresProductId == id).FirstOrDefaultAsync();
            if (deck != null)
            {
                dto.Description = deck.Description;
                dto.CoverImageUrl = deck.CoverImageUrl;
                dto.CardCount = deck.Cards.Count;
            }
        }

        return dto;
    }
}
