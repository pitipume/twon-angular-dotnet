using MediatR;
using Twon.Application.Common;
using Twon.Application.Payment.Managers;

namespace Twon.Application.Payment.Commands.RejectPayment;

public class RejectPaymentHandler(PaymentManager manager)
    : IRequestHandler<RejectPaymentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        RejectPaymentCommand request, CancellationToken cancellationToken)
        => await manager.RejectPaymentAsync(request.AdminId, request.OrderId, request.Reason);
}
