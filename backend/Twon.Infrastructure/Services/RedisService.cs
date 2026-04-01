using StackExchange.Redis;
using Twon.Application.Common.Interfaces;

namespace Twon.Infrastructure.Services;

public class RedisService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public Task SetAsync(string key, string value, TimeSpan expiry)
        => _db.StringSetAsync(key, value, expiry);

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? (string?)value : null;
    }

    public Task DeleteAsync(string key) => _db.KeyDeleteAsync(key);

    public async Task<long> IncrementAsync(string key, TimeSpan expiry)
    {
        var count = await _db.StringIncrementAsync(key);
        if (count == 1) await _db.KeyExpireAsync(key, expiry);
        return count;
    }
}
