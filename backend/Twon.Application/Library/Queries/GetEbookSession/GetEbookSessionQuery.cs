using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Library.Queries.GetEbookSession;

public record GetEbookSessionQuery(string UserId, string ProductId)
    : IRequest<BaseResult<EbookSessionDto>>;

public class EbookSessionDto
{
    public string ProductId { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
