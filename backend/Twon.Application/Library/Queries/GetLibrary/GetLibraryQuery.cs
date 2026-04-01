using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Library.Queries.GetLibrary;

public record GetLibraryQuery(string UserId) : IRequest<BaseResult<List<LibraryItemDto>>>;

public class LibraryItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
    public LibraryProductDto? Product { get; set; }
}

public class LibraryProductDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal PriceTHB { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Author { get; set; }
    public int? CardCount { get; set; }
}
