using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.Managers;

namespace Twon.Application.Auth.Commands.InitiateRegister;

public class InitiateRegisterHandler(AuthManager manager)
    : IRequestHandler<InitiateRegisterCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        InitiateRegisterCommand request, CancellationToken cancellationToken)
    {
        return await manager.InitiateRegisterAsync(request.Email, request.DisplayName, request.Password);
    }
}
