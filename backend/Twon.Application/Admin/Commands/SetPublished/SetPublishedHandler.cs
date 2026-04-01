using MediatR;
using Twon.Application.Common;
using Twon.Application.Admin.Managers;

namespace Twon.Application.Admin.Commands.SetPublished;

public class SetPublishedHandler(AdminManager manager)
    : IRequestHandler<SetPublishedCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        SetPublishedCommand request, CancellationToken cancellationToken)
        => await manager.SetPublishedAsync(request.ProductId, request.IsPublished);
}
