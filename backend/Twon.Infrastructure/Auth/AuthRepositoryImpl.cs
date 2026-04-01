using Microsoft.EntityFrameworkCore;
using Twon.Application.Auth.Repositories;
using Twon.Domain.Entities;
using Twon.Infrastructure.Persistence;

namespace Twon.Infrastructure.Auth;

public class AuthRepositoryImpl(TwonDbContext db) : IAuthRepository
{
    public Task<User?> FindByEmailAsync(string email)
        => db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateUserAsync(string email, string displayName, string passwordHash)
    {
        var user = new User { Email = email, DisplayName = displayName, PasswordHash = passwordHash };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash)
        => db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow);

    public async Task RevokeTokenAsync(Guid tokenId)
    {
        var token = await db.RefreshTokens.FindAsync(tokenId);
        if (token != null) { token.RevokedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
    }

    public async Task RevokeTokenByHashAsync(string tokenHash)
    {
        var token = await db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
        if (token != null) { token.RevokedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
    }

    public async Task SaveRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresAt)
    {
        db.RefreshTokens.Add(new RefreshToken { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt });
        await db.SaveChangesAsync();
    }
}
