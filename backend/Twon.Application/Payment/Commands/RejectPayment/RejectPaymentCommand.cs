using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Payment.Commands.RejectPayment;

public record RejectPaymentCommand(string AdminId, string OrderId, string Reason)
    : IRequest<BaseResult<object>>;
