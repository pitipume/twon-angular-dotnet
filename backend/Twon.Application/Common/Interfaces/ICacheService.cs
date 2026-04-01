namespace Twon.Application.Common.Interfaces;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan expiry);
    Task<string?> GetAsync(string key);
    Task DeleteAsync(string key);
    Task<long> IncrementAsync(string key, TimeSpan expiry);
}
