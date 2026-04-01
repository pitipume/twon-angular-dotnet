using Twon.Application.Library.Queries.GetLibrary;
using Twon.Application.Library.Queries.GetEbookSession;
using Twon.Application.Library.Queries.GetTarotSession;
using Twon.Application.Library.Repositories;

namespace Twon.Application.Library.Services;

public class LibraryService(LibraryRepository repository)
{
    public Task<List<LibraryItemDto>> GetLibraryItemsAsync(string userId)
        => repository.FindLibraryItemsAsync(userId);

    public Task<bool> UserOwnsProductAsync(string userId, string productId)
        => repository.UserOwnsProductAsync(userId, productId);

    public Task<EbookMetadata?> GetEbookMetadataAsync(string productId)
        => repository.FindEbookMetadataAsync(productId);

    public Task<TarotDeckMetadata?> GetTarotDeckMetadataAsync(string productId)
        => repository.FindTarotDeckMetadataAsync(productId);

    public Task<ReadingProgress?> GetProgressAsync(string userId, string productId)
        => repository.FindProgressAsync(userId, productId);

    public Task SaveProgressAsync(string userId, string productId, int page)
        => repository.SaveProgressAsync(userId, productId, page);
}

// Metadata types returned from MongoDB
public record EbookMetadata(string FileKey, int TotalPages);
public record TarotDeckMetadata(string Name, string? BackImageKey, List<TarotCardMeta> Cards);
public record TarotCardMeta(int CardNumber, string Name, string ImageKey,
    string UprightMeaning, string ReversedMeaning, List<string> Keywords);
public record ReadingProgress(int CurrentPage);
