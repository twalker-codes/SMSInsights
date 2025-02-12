using NSubstitute;
using SmsInsights.Interfaces;
using SmsInsights.Services;
using Xunit;
using System;

namespace SmsInsights.Tests.UnitTests;

public class RateLimiterServiceTests
{
    private readonly IRateLimiterService _rateLimiter;
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

    [Theory]
    [InlineData(9)]  // Just under sender limit
    [InlineData(10)] // At sender limit
    [InlineData(11)] // Just over sender limit
    public void CanSend_WithDifferentMessageCounts_HandlesLimitsCorrectly(int messageCount)
    {
        // Arrange
        var senderPhone = "+1234567890";
        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(messageCount < MaxMessagesPerSenderPerSec);
        
        // Act
        var result = _rateLimiter.CanSend(senderPhone);
        
        // Assert
        Assert.Equal(messageCount < MaxMessagesPerSenderPerSec, result);
    }

    [Theory]
    [InlineData(99)]  // Just under global limit
    [InlineData(100)] // At global limit
    [InlineData(101)] // Just over global limit
    public void CanSendGlobal_WithDifferentMessageCounts_HandlesLimitsCorrectly(int messageCount)
    {
        // Arrange
        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(messageCount < MaxMessagesGlobalPerSec);
        
        // Act
        var result = _rateLimiter.CanSendGlobal();
        
        // Assert
        Assert.Equal(messageCount < MaxMessagesGlobalPerSec, result);
    }

    [Fact]
    public void CanSend_WhenRedisFailure_HandlesDegradedState()
    {
        // Arrange
        var senderPhone = "+1234567890";
        _redisService.When(x => x.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>()))
            .Do(x => { throw new Exception("Redis connection failed"); });

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _rateLimiter.CanSend(senderPhone));
        Assert.Contains("Redis connection failed", exception.Message);
    }
} 