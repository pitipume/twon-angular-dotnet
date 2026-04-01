using MediatR;
using Twon.Application.Common;
using Twon.Application.Library.Managers;

namespace Twon.Application.Library.Queries.GetLibrary;

public class GetLibraryHandler(LibraryManager manager)
    : IRequestHandler<GetLibraryQuery, BaseResult<List<LibraryItemDto>>>
{
    public async Task<BaseResult<List<LibraryItemDto>>> Handle(
        GetLibraryQuery request, CancellationToken cancellationToken)
        => await manager.GetLibraryAsync(request.UserId);
}
