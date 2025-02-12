using SmsInsights.Models;
using SmsInsights.Validators;
using Xunit;

namespace SmsInsights.Tests.UnitTests;

public class MessageValidatorTests
{
    [Fact]
    public void ValidateRequest_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new SmsRequest
        {
            SenderPhoneNumber = "+1234567890",
            ReceiverPhoneNumber = "+0987654321",
            Message = "Test message"
        };

        // Act
        var (isValid, errorMessage) = MessageValidator.ValidateRequest(request);

        // Assert
        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    [Theory]
    [InlineData("", "+0987654321", "Test message", "Sender phone number is required.")]
    [InlineData(" ", "+0987654321", "Test message", "Sender phone number is required.")]
    [InlineData("+1234567890", "", "Test message", "Receiver phone number is required.")]
    [InlineData("+1234567890", " ", "Test message", "Receiver phone number is required.")]
    [InlineData("+1234567890", "+0987654321", "", "Message content is required.")]
    [InlineData("+1234567890", "+0987654321", " ", "Message content is required.")]
    public void ValidateRequest_WithInvalidRequest_ReturnsError(
        string senderPhone,
        string receiverPhone,
        string message,
        string expectedError)
    {
        // Arrange
        var request = new SmsRequest
        {
            SenderPhoneNumber = senderPhone,
            ReceiverPhoneNumber = receiverPhone,
            Message = message
        };

        // Act
        var (isValid, errorMessage) = MessageValidator.ValidateRequest(request);

        // Assert
        Assert.False(isValid);
        Assert.Equal(expectedError, errorMessage);
    }
} 