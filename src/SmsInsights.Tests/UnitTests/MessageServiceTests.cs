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
    private readonly IRateLimiter _rateLimiter;

    public MessageServiceTests()
    {
        _rateLimiter = Substitute.For<IRateLimiter>();
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
} 