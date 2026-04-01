using MediatR;
using Twon.Application.Common;

namespace Twon.Application.Admin.Commands.SetPublished;

public record SetPublishedCommand(string ProductId, bool IsPublished)
    : IRequest<BaseResult<object>>;
