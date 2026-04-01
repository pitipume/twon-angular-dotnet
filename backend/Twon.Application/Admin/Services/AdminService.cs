using Twon.Application.Admin.Commands.UploadEbook;
using Twon.Application.Admin.Commands.UploadTarotDeck;
using Twon.Application.Admin.Repositories;

namespace Twon.Application.Admin.Services;

public class AdminService(AdminRepository repository)
{
    public Task<string> CreateEbookDocAsync(UploadEbookCommand cmd)
        => repository.CreateEbookDocAsync(cmd);

    public Task<string> FinalizeEbookAsync(string mongoId, string fileKey,
        string coverImageUrl, UploadEbookCommand cmd)
        => repository.FinalizeEbookAsync(mongoId, fileKey, coverImageUrl, cmd);

    public Task<string> CreateTarotDocAsync(UploadTarotDeckCommand cmd)
        => repository.CreateTarotDocAsync(cmd);

    public Task<string> FinalizeTarotDeckAsync(string mongoId,
        List<(int Index, string Key, string Name)> cards,
        string coverUrl, string backKey, UploadTarotDeckCommand cmd)
        => repository.FinalizeTarotDeckAsync(mongoId, cards, coverUrl, backKey, cmd);

    public Task SetPublishedAsync(string productId, bool isPublished)
        => repository.SetPublishedAsync(productId, isPublished);

    public Task SetPaymentConfigAsync(string bankName, string accountName, string accountNumber)
        => repository.SetPaymentConfigAsync(bankName, accountName, accountNumber);

    public Task SetQrImageKeyAsync(string key)
        => repository.SetQrImageKeyAsync(key);
}
