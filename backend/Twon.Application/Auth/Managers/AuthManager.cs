using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Twon.Application.Common;
using Twon.Application.Common.Interfaces;
using Twon.Application.Auth.DTOs;
using Twon.Application.Auth.Services;
using Twon.Domain.Entities;
using Twon.Domain.Enums;

namespace Twon.Application.Auth.Managers;

public class AuthManager(
    AuthService service,
    ICacheService cache,
    IEmailService email,
    IConfiguration config)
{
    public async Task<BaseResult<object>> InitiateRegisterAsync(
        string emailAddr, string displayName, string password)
    {
        if (await service.FindUserByEmailAsync(emailAddr) is not null)
            return BaseResult<object>.Conflict("Email already registered.");

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var hash = HashString(otp);

        await cache.SetAsync($"otp:{emailAddr}", hash, TimeSpan.FromMinutes(5));
        await cache.SetAsync($"otp:pending:{emailAddr}",
            $"{displayName}|||{HashString(password)}", TimeSpan.FromMinutes(10));

        await email.SendOtpEmailAsync(emailAddr, displayName, otp);

        return BaseResult<object>.Success(null!, "OTP sent to your email.");
    }

    public async Task<BaseResult<AuthResponseDto>> VerifyRegisterAsync(string emailAddr, string otp)
    {
        var lockout = await cache.GetAsync($"otp:lockout:{emailAddr}");
        if (lockout is not null)
            return BaseResult<AuthResponseDto>.Failure("Too many attempts. Try again in 15 minutes.");

        var storedHash = await cache.GetAsync($"otp:{emailAddr}");
        if (storedHash is null)
            return BaseResult<AuthResponseDto>.Failure("OTP expired or not found.");

        if (storedHash != HashString(otp))
        {
            var attempts = await cache.IncrementAsync($"otp:attempts:{emailAddr}", TimeSpan.FromMinutes(5));
            if (attempts >= 3)
                await cache.SetAsync($"otp:lockout:{emailAddr}", "locked", TimeSpan.FromMinutes(15));
            return BaseResult<AuthResponseDto>.Failure("Invalid OTP.");
        }

        var pending = await cache.GetAsync($"otp:pending:{emailAddr}");
        if (pending is null)
            return BaseResult<AuthResponseDto>.Failure("Registration session expired.");

        var parts = pending.Split("|||");
        var displayName = parts[0];
        var passwordHash = parts[1];

        var user = await service.CreateUserAsync(emailAddr, displayName, passwordHash);

        await cache.DeleteAsync($"otp:{emailAddr}");
        await cache.DeleteAsync($"otp:pending:{emailAddr}");
        await cache.DeleteAsync($"otp:attempts:{emailAddr}");

        return await IssueTokensAsync(user);
    }

    public async Task<BaseResult<AuthResponseDto>> LoginAsync(string emailAddr, string password)
    {
        var user = await service.FindUserByEmailAsync(emailAddr);
        if (user is null || !VerifyPassword(password, user.PasswordHash))
            return BaseResult<AuthResponseDto>.Unauthorized("Invalid email or password.");

        if (!user.IsEmailVerified)
            return BaseResult<AuthResponseDto>.Failure("Email not verified.");

        return await IssueTokensAsync(user);
    }

    public async Task<BaseResult<AuthResponseDto>> RefreshTokenAsync(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return BaseResult<AuthResponseDto>.Unauthorized("No refresh token.");

        var hash = HashString(token);
        var stored = await service.FindRefreshTokenAsync(hash);

        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt < DateTime.UtcNow)
            return BaseResult<AuthResponseDto>.Unauthorized("Invalid or expired refresh token.");

        await service.RevokeRefreshTokenAsync(stored.Id);

        return await IssueTokensAsync(stored.User);
    }

    public async Task<BaseResult<object>> LogoutAsync(string userId, string? token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            var hash = HashString(token);
            await service.RevokeRefreshTokenByHashAsync(hash);
        }
        return BaseResult<object>.Success(null!);
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private async Task<BaseResult<AuthResponseDto>> IssueTokensAsync(User user)
    {
        var accessToken = GenerateJwt(user, config["JWT_SECRET"]!, TimeSpan.FromMinutes(15));
        var rawRefresh = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        var refreshHash = HashString(rawRefresh);

        await service.SaveRefreshTokenAsync(user.Id, refreshHash, DateTime.UtcNow.AddDays(7));

        return BaseResult<AuthResponseDto>.Success(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefresh,
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role.ToString(),
            }
        });
    }

    private static string GenerateJwt(User user, string secret, TimeSpan lifetime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashString(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }

    private static bool VerifyPassword(string password, string hash)
        => HashString(password) == hash;
}
