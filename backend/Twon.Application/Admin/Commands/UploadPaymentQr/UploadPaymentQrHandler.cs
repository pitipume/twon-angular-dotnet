using MediatR;
using Twon.Application.Common;
using Twon.Application.Admin.Managers;

namespace Twon.Application.Admin.Commands.UploadPaymentQr;

public class UploadPaymentQrHandler(AdminManager manager)
    : IRequestHandler<UploadPaymentQrCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        UploadPaymentQrCommand request, CancellationToken cancellationToken)
        => await manager.UploadPaymentQrAsync(request.File);
}
