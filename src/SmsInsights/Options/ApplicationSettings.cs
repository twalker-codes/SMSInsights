namespace SmsInsights.Options;

/// <summary>
/// Represents the application settings loaded from appsettings.json.
/// </summary>
public class ApplicationSettings
{
    public RateLimitSettings RateLimits { get; set; } = new()
    {
        MaxMessagesPerSenderPerSec = 10,  // Default value
        MaxMessagesGlobalPerSec = 100     // Default value
    };
    
    public RedisSettings Redis { get; set; } = new()
    {
        ConnectionString = "localhost:6379" // Default value
    };
    
    public ReactClientSettings ReactClient { get; set; } = new()
    {
        Origin = "http://localhost:4200"   // Default value
    };
}

/// <summary>
/// Represents the rate limit configuration settings.
/// </summary>
public class RateLimitSettings
{
    public int MaxMessagesPerSenderPerSec { get; set; }
    public int MaxMessagesGlobalPerSec { get; set; }
    public int MetricsWindowSeconds { get; set; } = 10;
}

/// <summary>
/// Represents the Redis configuration settings.
/// </summary>
public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}

/// <summary>
/// Represents the React client configuration settings.
/// </summary>
public class ReactClientSettings
{
    public string Origin { get; set; } = string.Empty;
}