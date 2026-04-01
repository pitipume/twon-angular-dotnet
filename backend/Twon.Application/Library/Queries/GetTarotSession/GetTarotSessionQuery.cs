using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Library.Queries.GetTarotSession;

public record GetTarotSessionQuery(string UserId, string ProductId)
    : IRequest<BaseResult<TarotSessionDto>>;

public class TarotSessionDto
{
    public string ProductId { get; set; } = string.Empty;
    public string DeckName { get; set; } = string.Empty;
    public string? BackImageUrl { get; set; }
    public List<TarotCardDto> Cards { get; set; } = [];
}

public class TarotCardDto
{
    public int CardNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string UprightMeaning { get; set; } = string.Empty;
    public string ReversedMeaning { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = [];
}
