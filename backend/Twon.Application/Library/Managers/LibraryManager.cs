using Twon.Application.Common;
using Twon.Application.Common.Interfaces;
using Twon.Application.Library.Queries.GetLibrary;
using Twon.Application.Library.Queries.GetEbookSession;
using Twon.Application.Library.Queries.GetTarotSession;
using Twon.Application.Library.Services;

namespace Twon.Application.Library.Managers;

public class LibraryManager(LibraryService service, IStorageService storage)
{
    public async Task<BaseResult<List<LibraryItemDto>>> GetLibraryAsync(string userId)
    {
        var items = await service.GetLibraryItemsAsync(userId);
        return BaseResult<List<LibraryItemDto>>.Success(items);
    }

    public async Task<BaseResult<EbookSessionDto>> GetEbookSessionAsync(string userId, string productId)
    {
        var owns = await service.UserOwnsProductAsync(userId, productId);
        if (!owns) return BaseResult<EbookSessionDto>.Unauthorized("You do not own this product.");

        var ebook = await service.GetEbookMetadataAsync(productId);
        if (ebook is null) return BaseResult<EbookSessionDto>.NotFound("Ebook not found.");

        var pdfUrl = await storage.GetSignedReadUrlAsync(ebook.FileKey, 2 * 60 * 60);
        var progress = await service.GetProgressAsync(userId, productId);

        return BaseResult<EbookSessionDto>.Success(new EbookSessionDto
        {
            ProductId = productId,
            PdfUrl = pdfUrl,
            TotalPages = ebook.TotalPages,
            CurrentPage = progress?.CurrentPage ?? 1,
        });
    }

    public async Task<BaseResult<TarotSessionDto>> GetTarotSessionAsync(string userId, string productId)
    {
        var owns = await service.UserOwnsProductAsync(userId, productId);
        if (!owns) return BaseResult<TarotSessionDto>.Unauthorized("You do not own this product.");

        var deck = await service.GetTarotDeckMetadataAsync(productId);
        if (deck is null) return BaseResult<TarotSessionDto>.NotFound("Tarot deck not found.");

        var backUrl = string.IsNullOrEmpty(deck.BackImageKey)
            ? null
            : await storage.GetSignedReadUrlAsync(deck.BackImageKey, 60 * 60);

        var cards = await Task.WhenAll(deck.Cards.Select(async c => new TarotCardDto
        {
            CardNumber = c.CardNumber,
            Name = c.Name,
            ImageUrl = await storage.GetSignedReadUrlAsync(c.ImageKey, 60 * 60),
            UprightMeaning = c.UprightMeaning,
            ReversedMeaning = c.ReversedMeaning,
            Keywords = c.Keywords,
        }));

        return BaseResult<TarotSessionDto>.Success(new TarotSessionDto
        {
            ProductId = productId,
            DeckName = deck.Name,
            BackImageUrl = backUrl,
            Cards = [.. cards],
        });
    }

    public async Task<BaseResult<object>> SaveProgressAsync(string userId, string productId, int page)
    {
        await service.SaveProgressAsync(userId, productId, page);
        return BaseResult<object>.Success(null!);
    }
}
