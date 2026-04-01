using MediatR;
using Twon.Application.Common;
using Twon.Application.Auth.DTOs;

namespace Twon.Application.Auth.Commands.VerifyRegister;

public record VerifyRegisterCommand(string Email, string Otp)
    : IRequest<BaseResult<AuthResponseDto>>;
