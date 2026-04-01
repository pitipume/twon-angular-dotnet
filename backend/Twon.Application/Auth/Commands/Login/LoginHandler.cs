using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;
using Twon.Application.Auth.Managers;

namespace Twon.Application.Auth.Commands.Login;

public class LoginHandler(AuthManager manager)
    : IRequestHandler<LoginCommand, BaseResult<AuthResponseDto>>
{
    public async Task<BaseResult<AuthResponseDto>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        return await manager.LoginAsync(request.Email, request.Password);
    }
}
