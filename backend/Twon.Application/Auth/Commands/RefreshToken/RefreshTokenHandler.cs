using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;
using Twon.Application.Auth.Managers;

namespace Twon.Application.Auth.Commands.RefreshToken;

public class RefreshTokenHandler(AuthManager manager)
    : IRequestHandler<RefreshTokenCommand, BaseResult<AuthResponseDto>>
{
    public async Task<BaseResult<AuthResponseDto>> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await manager.RefreshTokenAsync(request.Token);
    }
}
