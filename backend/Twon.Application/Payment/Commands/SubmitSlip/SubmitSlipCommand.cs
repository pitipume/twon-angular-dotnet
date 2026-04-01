using MediatR;
using Microsoft.AspNetCore.Http;
using Twon.Application.Common;

namespace Twon.Application.Payment.Commands.SubmitSlip;

public record SubmitSlipCommand(
    string UserId,
    string OrderId,
    IFormFile File,
    DateTime TransferredAt,
    string? Note
) : IRequest<BaseResult<object>>;
