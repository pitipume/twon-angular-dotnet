using Twon.Application.Library.Queries.GetLibrary;
using Twon.Application.Library.Services;

namespace Twon.Application.Library.Repositories;

public interface ILibraryRepository
{
    Task<List<LibraryItemDto>> FindLibraryItemsAsync(string userId);
    Task<bool> UserOwnsProductAsync(string userId, string productId);
    Task<EbookMetadata?> FindEbookMetadataAsync(string productId);
    Task<TarotDeckMetadata?> FindTarotDeckMetadataAsync(string productId);
    Task<ReadingProgress?> FindProgressAsync(string userId, string productId);
    Task SaveProgressAsync(string userId, string productId, int page);
}

public class LibraryRepository(ILibraryRepository inner)
{
    public Task<List<LibraryItemDto>> FindLibraryItemsAsync(string userId)
        => inner.FindLibraryItemsAsync(userId);
    public Task<bool> UserOwnsProductAsync(string userId, string productId)
        => inner.UserOwnsProductAsync(userId, productId);
    public Task<EbookMetadata?> FindEbookMetadataAsync(string productId)
        => inner.FindEbookMetadataAsync(productId);
    public Task<TarotDeckMetadata?> FindTarotDeckMetadataAsync(string productId)
        => inner.FindTarotDeckMetadataAsync(productId);
    public Task<ReadingProgress?> FindProgressAsync(string userId, string productId)
        => inner.FindProgressAsync(userId, productId);
    public Task SaveProgressAsync(string userId, string productId, int page)
        => inner.SaveProgressAsync(userId, productId, page);
}
