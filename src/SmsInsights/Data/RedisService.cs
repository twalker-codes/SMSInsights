using SmsInsights.Interfaces;
using StackExchange.Redis;

namespace SmsInsights.Data;

/// <summary>
/// Implements Redis operations using StackExchange.Redis.
/// </summary>
public class RedisService : IRedisService
{
    private readonly IDatabase _redisDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisService"/> class.
    /// </summary>
    /// <param name="redis">The Redis connection multiplexer.</param>
    public RedisService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    /// <inheritdoc/>
    public bool IncrementWithExpiration(string key, int maxLimit)
    {
        var count = _redisDb.StringIncrement(key);
        if (count == 1)
        {
            _redisDb.KeyExpire(key, TimeSpan.FromSeconds(1));
        }

        return count <= maxLimit;
    }

    /// <inheritdoc/>
    public void Set(string key, string value, TimeSpan expiration)
    {
        _redisDb.StringSet(key, value, expiration);
    }

    /// <inheritdoc/>
    public string? Get(string key)
    {
        return _redisDb.StringGet(key);
    }

    /// <inheritdoc/>
    public void Delete(string key)
    {
        _redisDb.KeyDelete(key);
    }
}
