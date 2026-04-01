using MediatR;
using Microsoft.AspNetCore.Http;
using Twon.Application.Common;

namespace Twon.Application.Admin.Commands.UploadEbook;

public record UploadEbookCommand(
    string AdminId,
    string Title,
    string Author,
    string Description,
    decimal PriceTHB,
    string Language,
    string? Categories,
    string? Tags,
    int PreviewPages,
    IFormFile Pdf,
    IFormFile? Cover
) : IRequest<BaseResult<UploadResultDto>>;

public class UploadResultDto
{
    public string ProductId { get; set; } = string.Empty;
    public string MongoId { get; set; } = string.Empty;
}
