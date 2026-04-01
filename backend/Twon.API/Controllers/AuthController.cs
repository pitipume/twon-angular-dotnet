using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twon.Application.Auth.Commands.InitiateRegister;
using Twon.Application.Auth.Commands.VerifyRegister;
using Twon.Application.Auth.Commands.Login;
using Twon.Application.Auth.Commands.RefreshToken;
using Twon.Application.Auth.Commands.Logout;

namespace Twon.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    // POST /api/auth/register/initiate
    [HttpPost("register/initiate")]
    public async Task<IActionResult> InitiateRegister([FromBody] InitiateRegisterCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    // POST /api/auth/register/verify
    [HttpPost("register/verify")]
    public async Task<IActionResult> VerifyRegister([FromBody] VerifyRegisterCommand command)
    {
        var result = await mediator.Send(command);
        if (result.Data?.RefreshToken is not null)
            SetRefreshTokenCookie(result.Data.RefreshToken);
        return Ok(result.WithoutRefreshToken());
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        if (result.Data?.RefreshToken is not null)
            SetRefreshTokenCookie(result.Data.RefreshToken);
        return Ok(result.WithoutRefreshToken());
    }

    // POST /api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        var result = await mediator.Send(new RefreshTokenCommand(refreshToken));
        if (result.Data?.RefreshToken is not null)
            SetRefreshTokenCookie(result.Data.RefreshToken);
        return Ok(result.WithoutRefreshToken());
    }

    // POST /api/auth/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await mediator.Send(new LogoutCommand(userId!, refreshToken));
        Response.Cookies.Delete("refresh_token");
        return Ok(result);
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refresh_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
        });
    }
}
