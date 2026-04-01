using MediatR;
using Twon.Application.Common;
using Twon.Application.Admin.Managers;

namespace Twon.Application.Admin.Commands.UploadEbook;

public class UploadEbookHandler(AdminManager manager)
    : IRequestHandler<UploadEbookCommand, BaseResult<UploadResultDto>>
{
    public async Task<BaseResult<UploadResultDto>> Handle(
        UploadEbookCommand request, CancellationToken cancellationToken)
        => await manager.UploadEbookAsync(request);
}
