using Twon.Application.Auth.Repositories;
using Twon.Domain.Entities;

namespace Twon.Application.Auth.Services;

public class AuthService(AuthRepository repository)
{
    public Task<User?> FindUserByEmailAsync(string email)
        => repository.FindByEmailAsync(email);

    public Task<User> CreateUserAsync(string email, string displayName, string passwordHash)
        => repository.CreateUserAsync(email, displayName, passwordHash);

    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash)
        => repository.FindRefreshTokenAsync(tokenHash);

    public Task RevokeRefreshTokenAsync(Guid tokenId)
        => repository.RevokeTokenAsync(tokenId);

    public Task RevokeRefreshTokenByHashAsync(string tokenHash)
        => repository.RevokeTokenByHashAsync(tokenHash);

    public Task SaveRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresAt)
        => repository.SaveRefreshTokenAsync(userId, tokenHash, expiresAt);
}
