using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Auth.Commands.InitiateRegister;

public record InitiateRegisterCommand(
    string Email,
    string DisplayName,
    string Password
) : IRequest<BaseResult<object>>;
