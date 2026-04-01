using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Twon.Application.Common;
using Twon.Application.Common.Interfaces;
using Twon.Application.Admin.Commands.UploadEbook;
using Twon.Application.Admin.Commands.UploadTarotDeck;
using Twon.Application.Admin.Services;

namespace Twon.Application.Admin.Managers;

public class AdminManager(AdminService service, IStorageService storage)
{
    public async Task<BaseResult<UploadResultDto>> UploadEbookAsync(UploadEbookCommand cmd)
    {
        var mongoId = await service.CreateEbookDocAsync(cmd);
        var fileKey = storage.BuildKey.EbookFile(mongoId);

        using var pdfStream = cmd.Pdf.OpenReadStream();
        await storage.UploadAsync(fileKey, pdfStream, "application/pdf");

        string coverImageUrl = string.Empty;
        if (cmd.Cover is not null)
        {
            using var coverStream = cmd.Cover.OpenReadStream();
            var coverKey = storage.BuildKey.EbookCover(mongoId);
            await storage.UploadAsync(coverKey, coverStream, cmd.Cover.ContentType);
            coverImageUrl = await storage.GetSignedReadUrlAsync(coverKey, 365 * 24 * 60 * 60);
        }

        var productId = await service.FinalizeEbookAsync(mongoId, fileKey, coverImageUrl, cmd);
        return BaseResult<UploadResultDto>.Success(new UploadResultDto
        {
            ProductId = productId,
            MongoId = mongoId,
        });
    }

    public async Task<BaseResult<UploadTarotResultDto>> UploadTarotDeckAsync(UploadTarotDeckCommand cmd)
    {
        var mongoId = await service.CreateTarotDocAsync(cmd);

        // Extract ZIP and upload each card
        using var zipStream = cmd.Zip.OpenReadStream();
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var entries = zip.Entries
            .Where(e => e.Name.EndsWith(".webp") || e.Name.EndsWith(".jpg") || e.Name.EndsWith(".png"))
            .OrderBy(e => e.Name)
            .ToList();

        var cards = new List<(int Index, string Key, string Name)>();
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var key = storage.BuildKey.TarotCard(mongoId, i);
            using var entryStream = entry.Open();
            await storage.UploadAsync(key, entryStream, "image/webp");

            var namePart = entry.Name.Replace(System.Text.RegularExpressions.Regex.Match(entry.Name, @"^\d+_").Value, "")
                .Replace(".webp", "").Replace(".jpg", "").Replace(".png", "");
            var cardName = string.Join(" ", namePart.Split('_').Select(w =>
                char.ToUpper(w[0]) + w[1..]));

            cards.Add((i, key, cardName));
        }

        string coverUrl = string.Empty, backKey = string.Empty;
        if (cmd.Cover is not null)
        {
            var coverKey = storage.BuildKey.TarotCover(mongoId);
            using var cs = cmd.Cover.OpenReadStream();
            await storage.UploadAsync(coverKey, cs, cmd.Cover.ContentType);
            coverUrl = await storage.GetSignedReadUrlAsync(coverKey, 365 * 24 * 60 * 60);
        }
        if (cmd.Back is not null)
        {
            backKey = storage.BuildKey.TarotBack(mongoId);
            using var bs = cmd.Back.OpenReadStream();
            await storage.UploadAsync(backKey, bs, cmd.Back.ContentType);
        }

        var productId = await service.FinalizeTarotDeckAsync(mongoId, cards, coverUrl, backKey, cmd);
        return BaseResult<UploadTarotResultDto>.Success(new UploadTarotResultDto
        {
            ProductId = productId,
            MongoId = mongoId,
            CardCount = cards.Count,
        });
    }

    public async Task<BaseResult<object>> SetPublishedAsync(string productId, bool isPublished)
    {
        await service.SetPublishedAsync(productId, isPublished);
        return BaseResult<object>.Success(null!);
    }

    public async Task<BaseResult<object>> SetPaymentConfigAsync(
        string bankName, string accountName, string accountNumber)
    {
        await service.SetPaymentConfigAsync(bankName, accountName, accountNumber);
        return BaseResult<object>.Success(null!);
    }

    public async Task<BaseResult<object>> UploadPaymentQrAsync(IFormFile file)
    {
        const string key = "payment-config/qr.webp";
        using var stream = file.OpenReadStream();
        await storage.UploadAsync(key, stream, file.ContentType);
        await service.SetQrImageKeyAsync(key);
        return BaseResult<object>.Success(null!, "QR image uploaded.");
    }
}
