using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Admin.Commands.SetPaymentConfig;

public record SetPaymentConfigCommand(string BankName, string AccountName, string AccountNumber)
    : IRequest<BaseResult<object>>;
