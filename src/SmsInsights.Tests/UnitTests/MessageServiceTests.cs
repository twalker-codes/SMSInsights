using NSubstitute;
using SmsInsights.Interfaces;
using SmsInsights.Models;
using SmsInsights.Options;
using SmsInsights.Services;
using Xunit;

namespace SmsInsights.Tests.UnitTests;

public class MessageServiceTests
{
    private readonly IMessageService _messageService;
    private readonly IRateLimiterService _rateLimiter;

    public MessageServiceTests()
    {
        _rateLimiter = Substitute.For<IRateLimiterService>();
        _messageService = new MessageService(_rateLimiter);
    }

    [Fact]
    public void CanSendMessage_WhenBothLimitsAllowed_ReturnsTrue()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);

        // Act
        var result = _messageService.CanSendMessage(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanSendMessage_WhenGlobalLimitExceeded_ReturnsFalse()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(false);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);

        // Act
        var result = _messageService.CanSendMessage(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSendMessage_WhenSenderLimitExceeded_ReturnsFalse()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(false);

        // Act
        var result = _messageService.CanSendMessage(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SendMessage_WhenRateLimitNotExceeded_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);

        // Act
        var result = _messageService.SendMessage(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ApiResponseMessages.MessageSentSuccess, result.Message);
    }

    [Fact]
    public void SendMessage_WhenRateLimitExceeded_ReturnsFailureResponse()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(false);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);

        // Act
        var result = _messageService.SendMessage(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ApiResponseMessages.RateLimitExceeded, result.Message);
    }

    [Fact]
    public void SendMessage_WhenApproachingGlobalLimit_StillSucceeds()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);
        _rateLimiter.GetGlobalUsagePercentage().Returns(95); // 95% of limit

        // Act
        var result = _messageService.SendMessage(request);

        // Assert
        Assert.True(result.Success);
        // Verify warning or metrics are logged (if implemented)
    }

    [Fact]
    public void SendMessage_WhenApproachingSenderLimit_StillSucceeds()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);
        _rateLimiter.GetSenderUsagePercentage(request.SenderPhoneNumber).Returns(90); // 90% of limit

        // Act
        var result = _messageService.SendMessage(request);

        // Assert
        Assert.True(result.Success);
        // Verify warning or metrics are logged (if implemented)
    }

    [Fact]
    public void SendMessage_WhenBothLimitsApproaching_StillSucceeds()
    {
        // Arrange
        var request = new SmsRequest { SenderPhoneNumber = "+1234567890" };
        _rateLimiter.CanSendGlobal().Returns(true);
        _rateLimiter.CanSend(request.SenderPhoneNumber).Returns(true);
        _rateLimiter.GetGlobalUsagePercentage().Returns(95);
        _rateLimiter.GetSenderUsagePercentage(request.SenderPhoneNumber).Returns(90);

        // Act
        var result = _messageService.SendMessage(request);

        // Assert
        Assert.True(result.Success);
        // Verify appropriate warnings or metrics are logged (if implemented)
    }
} 