using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twon.Application.Payment.Commands.SubmitSlip;
using Twon.Application.Payment.Commands.ApprovePayment;
using Twon.Application.Payment.Commands.RejectPayment;
using Twon.Application.Payment.Queries.GetPendingOrders;

namespace Twon.API.Controllers;

[Authorize]
[ApiController]
[Route("api/payment")]
public class PaymentController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    // POST /api/payment/slip
    [HttpPost("slip")]
    public async Task<IActionResult> SubmitSlip([FromForm] SubmitSlipCommand command)
    {
        var result = await mediator.Send(command with { UserId = UserId });
        return Ok(result);
    }

    // POST /api/payment/orders/:orderId/approve
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("orders/{orderId}/approve")]
    public async Task<IActionResult> ApprovePayment(string orderId)
    {
        var result = await mediator.Send(new ApprovePaymentCommand(UserId, orderId));
        return Ok(result);
    }

    // POST /api/payment/orders/:orderId/reject
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("orders/{orderId}/reject")]
    public async Task<IActionResult> RejectPayment(string orderId, [FromBody] RejectPaymentCommand command)
    {
        var result = await mediator.Send(command with { AdminId = UserId, OrderId = orderId });
        return Ok(result);
    }

    // GET /api/payment/orders/pending
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("orders/pending")]
    public async Task<IActionResult> GetPendingOrders()
    {
        var result = await mediator.Send(new GetPendingOrdersQuery());
        return Ok(result);
    }
}
