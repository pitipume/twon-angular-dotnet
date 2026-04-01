using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twon.Application.Store.Commands.CreateOrder;
using Twon.Application.Store.Queries.GetOrder;

namespace Twon.API.Controllers;

[Authorize]
[ApiController]
[Route("api/store")]
public class StoreController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    // POST /api/store/orders
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await mediator.Send(command with { UserId = UserId });
        return Ok(result);
    }

    // GET /api/store/orders/:orderId
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        var result = await mediator.Send(new GetOrderQuery(UserId, orderId));
        return Ok(result);
    }
}
