using MediatR;
using Twon.Application.Common;
using Twon.Application.Library.Managers;

namespace Twon.Application.Library.Queries.GetEbookSession;

public class GetEbookSessionHandler(LibraryManager manager)
    : IRequestHandler<GetEbookSessionQuery, BaseResult<EbookSessionDto>>
{
    public async Task<BaseResult<EbookSessionDto>> Handle(
        GetEbookSessionQuery request, CancellationToken cancellationToken)
        => await manager.GetEbookSessionAsync(request.UserId, request.ProductId);
}
