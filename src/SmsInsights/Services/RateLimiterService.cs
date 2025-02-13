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
    private readonly int _metricsWindowSeconds;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterService"/> class.
    /// </summary>
    /// <param name="redisService">The generic Redis service.</param>
    /// <param name="maxMessagesPerSenderPerSec">The max messages a sender can send per second.</param>
    /// <param name="maxMessagesGlobalPerSec">The max messages the system allows globally per second.</param>
    /// <param name="metricsWindowSeconds">The size of the metrics window in seconds.</param>
    public RateLimiterService(
        IRedisService redisService, 
        int maxMessagesPerSenderPerSec, 
        int maxMessagesGlobalPerSec,
        int metricsWindowSeconds = 10)  // Default to 10 seconds
    {
        _redisService = redisService;
        _maxMessagesPerSenderPerSec = maxMessagesPerSenderPerSec;
        _maxMessagesGlobalPerSec = maxMessagesGlobalPerSec;
        _metricsWindowSeconds = metricsWindowSeconds;
    }

    private IEnumerable<string> GetTimeWindows()
    {
        var now = DateTime.UtcNow;
        for (int i = 0; i < _metricsWindowSeconds; i++)
        {
            yield return now.AddSeconds(-i).ToString("yyyyMMddHHmmss");
        }
    }

    private string GetSenderKey(string senderPhoneNumber)
    {
        return $"rate_limit:{senderPhoneNumber}:{GetTimeWindows().First()}";
    }

    private string GetGlobalKey()
    {
        return $"global_rate_limit:{GetTimeWindows().First()}";
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
        var windows = GetTimeWindows();
        var counts = windows.Select(w => _redisService.GetCount($"global_rate_limit:{w}"));
        var totalCount = counts.Sum();
        return (int)((totalCount * 100.0) / (_maxMessagesGlobalPerSec * _metricsWindowSeconds));
    }

    public int GetSenderUsagePercentage(string senderNumber)
    {
        var windows = GetTimeWindows();
        var counts = windows.Select(w => _redisService.GetCount($"rate_limit:{senderNumber}:{w}"));
        var totalCount = counts.Sum();
        return (int)((totalCount * 100.0) / (_maxMessagesPerSenderPerSec * _metricsWindowSeconds));
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

    public void CleanupInactiveSenders(TimeSpan inactivityThreshold)
    {
        var cutoffTime = DateTime.UtcNow.Subtract(inactivityThreshold);
        var pattern = "rate_limit:*";
        var keys = _redisService.GetKeys(pattern);
        
        foreach (var key in keys)
        {
            var lastAccess = _redisService.GetLastAccessTime(key);
            if (lastAccess < cutoffTime)
            {
                _redisService.DeleteKey(key);
            }
        }
    }
}
