using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twon.Application.Library.Queries.GetLibrary;
using Twon.Application.Library.Queries.GetEbookSession;
using Twon.Application.Library.Queries.GetTarotSession;
using Twon.Application.Library.Commands.SaveProgress;

namespace Twon.API.Controllers;

[Authorize]
[ApiController]
[Route("api/library")]
public class LibraryController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    // GET /api/library
    [HttpGet]
    public async Task<IActionResult> GetLibrary()
    {
        var result = await mediator.Send(new GetLibraryQuery(UserId));
        return Ok(result);
    }

    // GET /api/library/ebook/:productId/session
    [HttpGet("ebook/{productId}/session")]
    public async Task<IActionResult> GetEbookSession(string productId)
    {
        var result = await mediator.Send(new GetEbookSessionQuery(UserId, productId));
        return Ok(result);
    }

    // POST /api/library/ebook/:productId/progress
    [HttpPost("ebook/{productId}/progress")]
    public async Task<IActionResult> SaveProgress(string productId, [FromBody] SaveProgressCommand command)
    {
        var result = await mediator.Send(command with { UserId = UserId, ProductId = productId });
        return Ok(result);
    }

    // GET /api/library/tarot/:productId/session
    [HttpGet("tarot/{productId}/session")]
    public async Task<IActionResult> GetTarotSession(string productId)
    {
        var result = await mediator.Send(new GetTarotSessionQuery(UserId, productId));
        return Ok(result);
    }
}
