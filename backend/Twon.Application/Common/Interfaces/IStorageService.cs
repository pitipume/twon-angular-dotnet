namespace Twon.Application.Common.Interfaces;

public interface IStorageService
{
    Task UploadAsync(string key, Stream content, string contentType);
    Task<string> GetSignedReadUrlAsync(string key, int expirySeconds);
    Task DeleteAsync(string key);
    StorageKeyBuilder BuildKey { get; }
}

public class StorageKeyBuilder
{
    public string EbookFile(string mongoId) => $"ebooks/{mongoId}/ebook.pdf";
    public string EbookCover(string mongoId) => $"ebooks/{mongoId}/cover.webp";
    public string TarotCard(string mongoId, int index) => $"tarot/{mongoId}/cards/{index}.webp";
    public string TarotCover(string mongoId) => $"tarot/{mongoId}/cover.webp";
    public string TarotBack(string mongoId) => $"tarot/{mongoId}/back.webp";
}
