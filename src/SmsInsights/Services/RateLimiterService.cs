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

    private string GetTimeWindow()
    {
        // Round down to the current second to ensure consistent key usage within the same second
        var now = DateTime.UtcNow;
        return now.ToString("yyyyMMddHHmmss");
    }

    private string GetSenderKey(string senderPhoneNumber)
    {
        return $"rate_limit:{senderPhoneNumber}:{GetTimeWindow()}";
    }

    private string GetGlobalKey()
    {
        return $"global_rate_limit:{GetTimeWindow()}";
    }

    /// <summary>
    /// Checks if a sender can send a message based on rate limits.
    /// </summary>
    /// <param name="senderPhoneNumber">The phone number of the sender.</param>
    /// <returns>True if the sender is within limits, otherwise false.</returns>
    public bool CanSend(string senderPhoneNumber)
    {
        var senderKey = GetSenderKey(senderPhoneNumber);
        return _redisService.IncrementWithExpiration(senderKey, _maxMessagesPerSenderPerSec);
    }

    /// <summary>
    /// Checks if the system-wide rate limit has been exceeded.
    /// </summary>
    /// <returns>True if within global rate limits, otherwise false.</returns>
    public bool CanSendGlobal()
    {
        var globalKey = GetGlobalKey();
        return _redisService.IncrementWithExpiration(globalKey, _maxMessagesGlobalPerSec);
    }

    public int GetGlobalUsagePercentage()
    {
        var currentWindow = GetTimeWindow();
        var previousWindow = DateTime.UtcNow.AddSeconds(-1).ToString("yyyyMMddHHmmss");
        
        var currentKey = $"global_rate_limit:{currentWindow}";
        var previousKey = $"global_rate_limit:{previousWindow}";
        
        var currentCount = _redisService.GetCount(currentKey);
        var previousCount = _redisService.GetCount(previousKey);
        
        // Use the higher count between current and previous second
        var maxCount = Math.Max(currentCount, previousCount);
        return (int)((maxCount * 100.0) / _maxMessagesGlobalPerSec);
    }

    public int GetSenderUsagePercentage(string senderNumber)
    {
        var currentWindow = GetTimeWindow();
        var previousWindow = DateTime.UtcNow.AddSeconds(-1).ToString("yyyyMMddHHmmss");
        
        var currentKey = $"rate_limit:{senderNumber}:{currentWindow}";
        var previousKey = $"rate_limit:{senderNumber}:{previousWindow}";
        
        var currentCount = _redisService.GetCount(currentKey);
        var previousCount = _redisService.GetCount(previousKey);
        
        // Use the higher count between current and previous second
        var maxCount = Math.Max(currentCount, previousCount);
        return (int)((maxCount * 100.0) / _maxMessagesPerSenderPerSec);
    }

    public int GetCountForKey(string key)
    {
        return _redisService.GetCount(key);
    }

    public int GetMaxMessagesPerSender()
    {
        return _maxMessagesPerSenderPerSec;
    }

    public int GetMaxMessagesGlobal()
    {
        return _maxMessagesGlobalPerSec;
    }
}
