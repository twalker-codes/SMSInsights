namespace SmsInsights.Interfaces;

/// <summary>
/// Defines generic operations for interacting with Redis.
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Increments a Redis key with expiration.
    /// </summary>
    /// <param name="key">The Redis key.</param>
    /// <param name="maxLimit">The maximum allowed count.</param>
    /// <returns>True if within the limit, otherwise false.</returns>
    bool IncrementWithExpiration(string key, int maxLimit);

    /// <summary>
    /// Stores a value in Redis with an expiration time.
    /// </summary>
    /// <param name="key">The Redis key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiration">The expiration time.</param>
    void Set(string key, string value, TimeSpan expiration);

    /// <summary>
    /// Retrieves a value from Redis by key.
    /// </summary>
    /// <param name="key">The Redis key.</param>
    /// <returns>The stored value or null if not found.</returns>
    string? Get(string key);

    /// <summary>
    /// Deletes a key from Redis.
    /// </summary>
    /// <param name="key">The Redis key.</param>
    void Delete(string key);

    /// <summary>
    /// Retrieves the count of a Redis key.
    /// </summary>
    /// <param name="key">The Redis key.</param>
    /// <returns>The count of the Redis key.</returns>
    int GetCount(string key);
}
