using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twon.Application.Admin.Commands.UploadEbook;
using Twon.Application.Admin.Commands.UploadTarotDeck;
using Twon.Application.Admin.Commands.SetPublished;
using Twon.Application.Admin.Commands.SetPaymentConfig;
using Twon.Application.Admin.Commands.UploadPaymentQr;
using System.Security.Claims;

namespace Twon.API.Controllers;

[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
[ApiController]
[Route("api/admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    // POST /api/admin/ebooks
    [HttpPost("ebooks")]
    public async Task<IActionResult> UploadEbook([FromForm] UploadEbookCommand command)
    {
        var result = await mediator.Send(command with { AdminId = UserId });
        return Ok(result);
    }

    // POST /api/admin/tarot-decks
    [HttpPost("tarot-decks")]
    public async Task<IActionResult> UploadTarotDeck([FromForm] UploadTarotDeckCommand command)
    {
        var result = await mediator.Send(command with { AdminId = UserId });
        return Ok(result);
    }

    // PATCH /api/admin/products/:id/publish
    [HttpPatch("products/{id}/publish")]
    public async Task<IActionResult> Publish(string id)
    {
        var result = await mediator.Send(new SetPublishedCommand(id, true));
        return Ok(result);
    }

    // PATCH /api/admin/products/:id/unpublish
    [HttpPatch("products/{id}/unpublish")]
    public async Task<IActionResult> Unpublish(string id)
    {
        var result = await mediator.Send(new SetPublishedCommand(id, false));
        return Ok(result);
    }

    // PUT /api/admin/payment-config
    [HttpPut("payment-config")]
    public async Task<IActionResult> SetPaymentConfig([FromBody] SetPaymentConfigCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    // POST /api/admin/payment-config/qr
    [HttpPost("payment-config/qr")]
    public async Task<IActionResult> UploadPaymentQr([FromForm] UploadPaymentQrCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
