using Twon.Application.Admin.Commands.UploadEbook;
using Twon.Application.Admin.Commands.UploadTarotDeck;

namespace Twon.Application.Admin.Repositories;

public interface IAdminRepository
{
    Task<string> CreateEbookDocAsync(UploadEbookCommand cmd);
    Task<string> FinalizeEbookAsync(string mongoId, string fileKey,
        string coverImageUrl, UploadEbookCommand cmd);
    Task<string> CreateTarotDocAsync(UploadTarotDeckCommand cmd);
    Task<string> FinalizeTarotDeckAsync(string mongoId,
        List<(int Index, string Key, string Name)> cards,
        string coverUrl, string backKey, UploadTarotDeckCommand cmd);
    Task SetPublishedAsync(string productId, bool isPublished);
    Task SetPaymentConfigAsync(string bankName, string accountName, string accountNumber);
    Task SetQrImageKeyAsync(string key);
}

public class AdminRepository(IAdminRepository inner)
{
    public Task<string> CreateEbookDocAsync(UploadEbookCommand cmd)
        => inner.CreateEbookDocAsync(cmd);
    public Task<string> FinalizeEbookAsync(string mongoId, string fileKey,
        string coverImageUrl, UploadEbookCommand cmd)
        => inner.FinalizeEbookAsync(mongoId, fileKey, coverImageUrl, cmd);
    public Task<string> CreateTarotDocAsync(UploadTarotDeckCommand cmd)
        => inner.CreateTarotDocAsync(cmd);
    public Task<string> FinalizeTarotDeckAsync(string mongoId,
        List<(int Index, string Key, string Name)> cards,
        string coverUrl, string backKey, UploadTarotDeckCommand cmd)
        => inner.FinalizeTarotDeckAsync(mongoId, cards, coverUrl, backKey, cmd);
    public Task SetPublishedAsync(string productId, bool isPublished)
        => inner.SetPublishedAsync(productId, isPublished);
    public Task SetPaymentConfigAsync(string bankName, string accountName, string accountNumber)
        => inner.SetPaymentConfigAsync(bankName, accountName, accountNumber);
    public Task SetQrImageKeyAsync(string key)
        => inner.SetQrImageKeyAsync(key);
}
