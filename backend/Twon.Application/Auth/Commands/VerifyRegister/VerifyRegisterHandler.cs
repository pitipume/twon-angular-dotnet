using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;
using Twon.Application.Auth.Managers;

namespace Twon.Application.Auth.Commands.VerifyRegister;

public class VerifyRegisterHandler(AuthManager manager)
    : IRequestHandler<VerifyRegisterCommand, BaseResult<AuthResponseDto>>
{
    public async Task<BaseResult<AuthResponseDto>> Handle(
        VerifyRegisterCommand request, CancellationToken cancellationToken)
    {
        return await manager.VerifyRegisterAsync(request.Email, request.Otp);
    }
}
