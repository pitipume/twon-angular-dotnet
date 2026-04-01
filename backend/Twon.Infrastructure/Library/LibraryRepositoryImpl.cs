using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Twon.Application.Library.Queries.GetLibrary;
using Twon.Application.Library.Repositories;
using Twon.Application.Library.Services;
using Twon.Domain.Enums;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Library;

public class LibraryRepositoryImpl(TwonDbContext db, MongoDbContext mongo) : ILibraryRepository
{
    public async Task<List<LibraryItemDto>> FindLibraryItemsAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid)) return [];

        var items = await db.LibraryItems
            .Include(li => li.Product)
            .Where(li => li.UserId == userGuid)
            .ToListAsync();

        var result = new List<LibraryItemDto>();
        foreach (var item in items)
        {
            var dto = new LibraryItemDto
            {
                Id = item.Id.ToString(),
                ProductId = item.ProductId.ToString(),
                GrantedAt = item.GrantedAt,
                Product = new LibraryProductDto
                {
                    Id = item.Product.Id.ToString(),
                    ProductType = item.Product.ProductType.ToString(),
                    Title = item.Product.Title,
                    PriceTHB = item.Product.PriceTHB,
                }
            };

            if (item.Product.ProductType == ProductType.EBOOK)
            {
                var ebook = await mongo.EbookDocuments
                    .Find(e => e.PostgresProductId == item.ProductId.ToString()).FirstOrDefaultAsync();
                if (ebook != null)
                {
                    dto.Product.CoverImageUrl = ebook.CoverImageUrl;
                    dto.Product.Author = ebook.Author;
                }
            }
            else if (item.Product.ProductType == ProductType.TAROT_DECK)
            {
                var deck = await mongo.TarotDeckDocuments
                    .Find(d => d.PostgresProductId == item.ProductId.ToString()).FirstOrDefaultAsync();
                if (deck != null)
                {
                    dto.Product.CoverImageUrl = deck.CoverImageUrl;
                    dto.Product.CardCount = deck.Cards.Count;
                }
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<bool> UserOwnsProductAsync(string userId, string productId)
    {
        if (!Guid.TryParse(userId, out var userGuid) || !Guid.TryParse(productId, out var productGuid))
            return false;
        return await db.LibraryItems
            .AnyAsync(li => li.UserId == userGuid && li.ProductId == productGuid);
    }

    public async Task<EbookMetadata?> FindEbookMetadataAsync(string productId)
    {
        var ebook = await mongo.EbookDocuments
            .Find(e => e.PostgresProductId == productId).FirstOrDefaultAsync();
        return ebook == null ? null : new EbookMetadata(ebook.FileKey, ebook.TotalPages);
    }

    public async Task<TarotDeckMetadata?> FindTarotDeckMetadataAsync(string productId)
    {
        var deck = await mongo.TarotDeckDocuments
            .Find(d => d.PostgresProductId == productId).FirstOrDefaultAsync();
        if (deck == null) return null;

        var cards = deck.Cards.Select(c => new TarotCardMeta(
            c.CardNumber, c.Name, c.ImageKey,
            c.UprightMeaning, c.ReversedMeaning, c.Keywords)).ToList();

        return new TarotDeckMetadata(deck.Name, deck.BackImageKey, cards);
    }

    public async Task<ReadingProgress?> FindProgressAsync(string userId, string productId)
    {
        var prog = await mongo.ReadingProgressDocuments
            .Find(p => p.UserId == userId && p.ProductId == productId).FirstOrDefaultAsync();
        return prog == null ? null : new ReadingProgress(prog.CurrentPage);
    }

    public async Task SaveProgressAsync(string userId, string productId, int page)
    {
        var filter = Builders<ReadingProgressDocument>.Filter
            .Where(p => p.UserId == userId && p.ProductId == productId);
        var update = Builders<ReadingProgressDocument>.Update
            .Set(p => p.CurrentPage, page)
            .Set(p => p.UpdatedAt, DateTime.UtcNow)
            .SetOnInsert(p => p.UserId, userId)
            .SetOnInsert(p => p.ProductId, productId);
        await mongo.ReadingProgressDocuments.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
