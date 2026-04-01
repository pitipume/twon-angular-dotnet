using MediatR;
using Twon.Application.Common;
using Twon.Application.Library.Managers;

namespace Twon.Application.Library.Commands.SaveProgress;

public class SaveProgressHandler(LibraryManager manager)
    : IRequestHandler<SaveProgressCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        SaveProgressCommand request, CancellationToken cancellationToken)
        => await manager.SaveProgressAsync(request.UserId, request.ProductId, request.CurrentPage);
}
