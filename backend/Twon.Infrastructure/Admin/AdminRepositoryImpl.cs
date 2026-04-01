using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Twon.Application.Admin.Commands.UploadEbook;
using Twon.Application.Admin.Commands.UploadTarotDeck;
using Twon.Application.Admin.Repositories;
using Twon.Domain.Entities;
using Twon.Domain.Enums;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Admin;

public class AdminRepositoryImpl(TwonDbContext db, MongoDbContext mongo) : IAdminRepository
{
    public async Task<string> CreateEbookDocAsync(UploadEbookCommand cmd)
    {
        var doc = new EbookDocument
        {
            PostgresProductId = string.Empty,  // filled after Postgres insert
            Title = cmd.Title,
            Author = cmd.Author,
            Description = cmd.Description,
            Language = cmd.Language,
            Categories = cmd.Categories?.Split(',').Select(c => c.Trim()).Where(c => c.Length > 0).ToList() ?? [],
            Tags = cmd.Tags?.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).ToList() ?? [],
            FileKey = string.Empty,
            TotalPages = 0,
            PreviewPages = cmd.PreviewPages,
            IsPublished = false
        };

        await mongo.EbookDocuments.InsertOneAsync(doc);
        return doc.Id;
    }

    public async Task<string> FinalizeEbookAsync(string mongoId, string fileKey,
        string coverImageUrl, UploadEbookCommand cmd)
    {
        // Create Postgres product
        var product = new Product
        {
            MongoRefId = mongoId,
            ProductType = ProductType.EBOOK,
            Title = cmd.Title,
            PriceTHB = cmd.PriceTHB,
            IsPublished = false
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        // Update MongoDB doc with real keys + postgres reference
        var filter = Builders<EbookDocument>.Filter.Eq(e => e.Id, mongoId);
        var update = Builders<EbookDocument>.Update
            .Set(e => e.PostgresProductId, product.Id.ToString())
            .Set(e => e.FileKey, fileKey)
            .Set(e => e.CoverImageUrl, coverImageUrl);
        await mongo.EbookDocuments.UpdateOneAsync(filter, update);

        return product.Id.ToString();
    }

    public async Task<string> CreateTarotDocAsync(UploadTarotDeckCommand cmd)
    {
        var doc = new TarotDeckDocument
        {
            PostgresProductId = string.Empty,
            Name = cmd.Name,
            Description = cmd.Description,
            Cards = [],
            IsPublished = false
        };
        await mongo.TarotDeckDocuments.InsertOneAsync(doc);
        return doc.Id;
    }

    public async Task<string> FinalizeTarotDeckAsync(string mongoId,
        List<(int Index, string Key, string Name)> cards,
        string coverUrl, string backKey, UploadTarotDeckCommand cmd)
    {
        var product = new Product
        {
            MongoRefId = mongoId,
            ProductType = ProductType.TAROT_DECK,
            Title = cmd.Name,
            PriceTHB = cmd.PriceTHB,
            IsPublished = false
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var cardDocs = cards.Select(c => new TarotCardDocument
        {
            CardNumber = c.Index,
            Name = c.Name,
            ImageKey = c.Key,
            UprightMeaning = string.Empty,
            ReversedMeaning = string.Empty,
            Keywords = []
        }).ToList();

        var filter = Builders<TarotDeckDocument>.Filter.Eq(d => d.Id, mongoId);
        var update = Builders<TarotDeckDocument>.Update
            .Set(d => d.PostgresProductId, product.Id.ToString())
            .Set(d => d.Cards, cardDocs)
            .Set(d => d.CoverImageUrl, coverUrl)
            .Set(d => d.BackImageKey, backKey);
        await mongo.TarotDeckDocuments.UpdateOneAsync(filter, update);

        return product.Id.ToString();
    }

    public async Task SetPublishedAsync(string productId, bool isPublished)
    {
        if (!Guid.TryParse(productId, out var guid)) return;
        var product = await db.Products.FindAsync(guid);
        if (product == null) return;

        product.IsPublished = isPublished;
        product.UpdatedAt = DateTime.UtcNow;

        if (product.ProductType == ProductType.EBOOK)
        {
            await mongo.EbookDocuments.UpdateOneAsync(
                Builders<EbookDocument>.Filter.Eq(e => e.PostgresProductId, productId),
                Builders<EbookDocument>.Update.Set(e => e.IsPublished, isPublished));
        }
        else if (product.ProductType == ProductType.TAROT_DECK)
        {
            await mongo.TarotDeckDocuments.UpdateOneAsync(
                Builders<TarotDeckDocument>.Filter.Eq(d => d.PostgresProductId, productId),
                Builders<TarotDeckDocument>.Update.Set(d => d.IsPublished, isPublished));
        }

        await db.SaveChangesAsync();
    }

    public async Task SetPaymentConfigAsync(string bankName, string accountName, string accountNumber)
    {
        var config = await db.PaymentConfigs.FindAsync("singleton");
        if (config == null)
        {
            db.PaymentConfigs.Add(new PaymentConfig
            {
                BankName = bankName, AccountName = accountName, AccountNumber = accountNumber
            });
        }
        else
        {
            config.BankName = bankName;
            config.AccountName = accountName;
            config.AccountNumber = accountNumber;
            config.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
    }

    public async Task SetQrImageKeyAsync(string key)
    {
        var config = await db.PaymentConfigs.FindAsync("singleton");
        if (config != null) { config.QrImageKey = key; config.UpdatedAt = DateTime.UtcNow; }
        await db.SaveChangesAsync();
    }
}
