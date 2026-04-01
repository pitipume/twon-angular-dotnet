using MediatR;
using Twon.Application.Common;
using Twon.Application.Payment.Managers;

namespace Twon.Application.Payment.Commands.ApprovePayment;

public class ApprovePaymentHandler(PaymentManager manager)
    : IRequestHandler<ApprovePaymentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        ApprovePaymentCommand request, CancellationToken cancellationToken)
        => await manager.ApprovePaymentAsync(request.AdminId, request.OrderId);
}
