using MediatR;
using Twon.Application.Common;
using Twon.Application.Payment.Managers;

namespace Twon.Application.Payment.Commands.SubmitSlip;

public class SubmitSlipHandler(PaymentManager manager)
    : IRequestHandler<SubmitSlipCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        SubmitSlipCommand request, CancellationToken cancellationToken)
        => await manager.SubmitSlipAsync(
            request.UserId, request.OrderId,
            request.File, request.TransferredAt, request.Note);
}
