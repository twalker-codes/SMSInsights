using NSubstitute;
using SmsInsights.Interfaces;
using SmsInsights.Services;
using Xunit;

namespace SmsInsights.Tests.UnitTests;

public class RateLimiterServiceTests
{
    private readonly IRateLimiter _rateLimiter;
    private readonly IRedisService _redisService;
    private const int MaxMessagesPerSenderPerSec = 10;
    private const int MaxMessagesGlobalPerSec = 100;

    public RateLimiterServiceTests()
    {
        _redisService = Substitute.For<IRedisService>();
        _rateLimiter = new RateLimiterService(_redisService, MaxMessagesPerSenderPerSec, MaxMessagesGlobalPerSec);
    }

    [Fact]
    public void CanSend_WhenUnderLimit_ReturnsTrue()
    {
        // Arrange
        var senderPhone = "+1234567890";
        _redisService.IncrementWithExpiration(Arg.Any<string>(), MaxMessagesPerSenderPerSec).Returns(true);

        // Act
        var result = _rateLimiter.CanSend(senderPhone);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanSend_WhenOverLimit_ReturnsFalse()
    {
        // Arrange
        var senderPhone = "+1234567890";
        _redisService.IncrementWithExpiration(Arg.Any<string>(), MaxMessagesPerSenderPerSec).Returns(false);

        // Act
        var result = _rateLimiter.CanSend(senderPhone);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSendGlobal_WhenUnderLimit_ReturnsTrue()
    {
        // Arrange
        _redisService.IncrementWithExpiration(Arg.Any<string>(), MaxMessagesGlobalPerSec).Returns(true);

        // Act
        var result = _rateLimiter.CanSendGlobal();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanSendGlobal_WhenOverLimit_ReturnsFalse()
    {
        // Arrange
        _redisService.IncrementWithExpiration(Arg.Any<string>(), MaxMessagesGlobalPerSec).Returns(false);

        // Act
        var result = _rateLimiter.CanSendGlobal();

        // Assert
        Assert.False(result);
    }
} 