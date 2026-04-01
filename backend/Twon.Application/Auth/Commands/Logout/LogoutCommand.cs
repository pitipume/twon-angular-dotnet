using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Auth.Commands.Logout;

public record LogoutCommand(string UserId, string? RefreshToken)
    : IRequest<BaseResult<object>>;
