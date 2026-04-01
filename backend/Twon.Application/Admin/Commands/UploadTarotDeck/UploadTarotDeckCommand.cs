using MediatR;
using Microsoft.AspNetCore.Http;
using Twon.Application.Common;
using Twon.Application.Admin.Commands.UploadEbook;

namespace Twon.Application.Admin.Commands.UploadTarotDeck;

public record UploadTarotDeckCommand(
    string AdminId,
    string Name,
    string Description,
    decimal PriceTHB,
    IFormFile Zip,
    IFormFile? Cover,
    IFormFile? Back
) : IRequest<BaseResult<UploadTarotResultDto>>;

public class UploadTarotResultDto
{
    public string ProductId { get; set; } = string.Empty;
    public string MongoId { get; set; } = string.Empty;
    public int CardCount { get; set; }
}
