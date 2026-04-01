using Twon.Domain.Entities;

namespace Twon.Application.Auth.Repositories;

// Interface — implemented in Twon.Infrastructure
public interface IAuthRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User> CreateUserAsync(string email, string displayName, string passwordHash);
    Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash);
    Task RevokeTokenAsync(Guid tokenId);
    Task RevokeTokenByHashAsync(string tokenHash);
    Task SaveRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresAt);
}

// Concrete class — delegates to the interface (registered via DI)
public class AuthRepository(IAuthRepository inner)
{
    public Task<User?> FindByEmailAsync(string email) => inner.FindByEmailAsync(email);
    public Task<User> CreateUserAsync(string email, string displayName, string passwordHash)
        => inner.CreateUserAsync(email, displayName, passwordHash);
    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash)
        => inner.FindRefreshTokenAsync(tokenHash);
    public Task RevokeTokenAsync(Guid tokenId) => inner.RevokeTokenAsync(tokenId);
    public Task RevokeTokenByHashAsync(string tokenHash) => inner.RevokeTokenByHashAsync(tokenHash);
    public Task SaveRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresAt)
        => inner.SaveRefreshTokenAsync(userId, tokenHash, expiresAt);
}
