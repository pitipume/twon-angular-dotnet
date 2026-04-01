using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;

namespace Twon.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<BaseResult<AuthResponseDto>>;
