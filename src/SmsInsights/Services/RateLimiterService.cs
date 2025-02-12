using SmsInsights.Interfaces;

namespace SmsInsights.Services;

/// <summary>
/// Implements rate limiting using Redis.
/// </summary>
public class RateLimiterService : IRateLimiterService
{
    private readonly IRedisService _redisService;
    private readonly int _maxMessagesPerSenderPerSec;
    private readonly int _maxMessagesGlobalPerSec;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterService"/> class.
    /// </summary>
    /// <param name="redisService">The generic Redis service.</param>
    /// <param name="maxMessagesPerSenderPerSec">The max messages a sender can send per second.</param>
    /// <param name="maxMessagesGlobalPerSec">The max messages the system allows globally per second.</param>
    public RateLimiterService(IRedisService redisService, int maxMessagesPerSenderPerSec, int maxMessagesGlobalPerSec)
    {
        _redisService = redisService;
        _maxMessagesPerSenderPerSec = maxMessagesPerSenderPerSec;
        _maxMessagesGlobalPerSec = maxMessagesGlobalPerSec;
    }

    /// <summary>
    /// Checks if a sender can send a message based on rate limits.
    /// </summary>
    /// <param name="senderPhoneNumber">The phone number of the sender.</param>
    /// <returns>True if the sender is within limits, otherwise false.</returns>
    public bool CanSend(string senderPhoneNumber)
    {
        var senderKey = $"rate_limit:{senderPhoneNumber}:{DateTime.UtcNow:yyyyMMddHHmmss}";
        return _redisService.IncrementWithExpiration(senderKey, _maxMessagesPerSenderPerSec);
    }

    /// <summary>
    /// Checks if the system-wide rate limit has been exceeded.
    /// </summary>
    /// <returns>True if within global rate limits, otherwise false.</returns>
    public bool CanSendGlobal()
    {
        var globalKey = $"global_rate_limit:{DateTime.UtcNow:yyyyMMddHHmmss}";
        return _redisService.IncrementWithExpiration(globalKey, _maxMessagesGlobalPerSec);
    }
}
