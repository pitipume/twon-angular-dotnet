using MediatR;
using Twon.Application.Common;
using Twon.Application.Library.Managers;

namespace Twon.Application.Library.Queries.GetTarotSession;

public class GetTarotSessionHandler(LibraryManager manager)
    : IRequestHandler<GetTarotSessionQuery, BaseResult<TarotSessionDto>>
{
    public async Task<BaseResult<TarotSessionDto>> Handle(
        GetTarotSessionQuery request, CancellationToken cancellationToken)
        => await manager.GetTarotSessionAsync(request.UserId, request.ProductId);
}
