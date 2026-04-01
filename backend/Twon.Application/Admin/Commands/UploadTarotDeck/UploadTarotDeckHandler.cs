using MediatR;
using Twon.Application.Common;
using Twon.Application.Admin.Managers;

namespace Twon.Application.Admin.Commands.UploadTarotDeck;

public class UploadTarotDeckHandler(AdminManager manager)
    : IRequestHandler<UploadTarotDeckCommand, BaseResult<UploadTarotResultDto>>
{
    public async Task<BaseResult<UploadTarotResultDto>> Handle(
        UploadTarotDeckCommand request, CancellationToken cancellationToken)
        => await manager.UploadTarotDeckAsync(request);
}
