using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;

namespace Twon.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string? Token)
    : IRequest<BaseResult<AuthResponseDto>>;
