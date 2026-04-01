using MediatR;
using Twon.Application.Common;
using Twon.Application.Admin.Managers;

namespace Twon.Application.Admin.Commands.SetPaymentConfig;

public class SetPaymentConfigHandler(AdminManager manager)
    : IRequestHandler<SetPaymentConfigCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        SetPaymentConfigCommand request, CancellationToken cancellationToken)
        => await manager.SetPaymentConfigAsync(request.BankName, request.AccountName, request.AccountNumber);
}
