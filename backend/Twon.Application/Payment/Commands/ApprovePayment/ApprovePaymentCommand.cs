using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Payment.Commands.ApprovePayment;

public record ApprovePaymentCommand(string AdminId, string OrderId)
    : IRequest<BaseResult<object>>;
