using Xunit;
using NSubstitute;
using SmsInsights.Interfaces;
using SmsInsights.Services;

namespace SmsInsights.Tests
{
    public class RateLimiterTests
    {
        private readonly IRateLimiterService _rateLimiter;
        private readonly IRedisService _redisService;

        public RateLimiterTests()
        {
            _redisService = Substitute.For<IRedisService>();
            _rateLimiter = new RateLimiterService(_redisService, 5, 20);
        }

        [Fact]
        public void WhenRateLimitExceeded_ShouldReturnFalse()
        {
            // Arrange
            var senderNumber = "+1234567890";
            _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
                        .Returns(false);

            // Act
            var result = _rateLimiter.CanSend(senderNumber);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WhenUnderRateLimit_ShouldReturnTrue()
        {
            // Arrange
            var senderNumber = "+1234567890";
            _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
                        .Returns(true);

            // Act
            var result = _rateLimiter.CanSend(senderNumber);

            // Assert
            Assert.True(result);
        }
    }
} 