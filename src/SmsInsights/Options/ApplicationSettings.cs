namespace SmsInsights.Options;

/// <summary>
/// Represents the application settings loaded from appsettings.json.
/// </summary>
public class ApplicationSettings
{
    public RateLimitSettings RateLimits { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
}

/// <summary>
/// Represents the rate limit configuration settings.
/// </summary>
public class RateLimitSettings
{
    public int MaxMessagesPerSenderPerSec { get; set; }
    public int MaxMessagesGlobalPerSec { get; set; }
}

/// <summary>
/// Represents the Redis configuration settings.
/// </summary>
public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}
