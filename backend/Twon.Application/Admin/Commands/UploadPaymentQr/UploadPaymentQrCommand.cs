using MediatR;
using Microsoft.AspNetCore.Http;
using Twon.Application.Common;

namespace Twon.Application.Admin.Commands.UploadPaymentQr;

public record UploadPaymentQrCommand(IFormFile File) : IRequest<BaseResult<object>>;
