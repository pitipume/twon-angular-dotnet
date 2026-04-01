using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Library.Commands.SaveProgress;

public record SaveProgressCommand(string UserId, string ProductId, int CurrentPage)
    : IRequest<BaseResult<object>>;
