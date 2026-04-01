using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.Managers;

namespace Twon.Application.Auth.Commands.Logout;

public class LogoutHandler(AuthManager manager)
    : IRequestHandler<LogoutCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(
        LogoutCommand request, CancellationToken cancellationToken)
    {
        return await manager.LogoutAsync(request.UserId, request.RefreshToken);
    }
}
