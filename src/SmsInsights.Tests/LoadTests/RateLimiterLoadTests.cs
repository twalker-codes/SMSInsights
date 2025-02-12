using System.Collections.Concurrent;
using System.Diagnostics;
using NSubstitute;
using SmsInsights.Data;
using SmsInsights.Services;
using SmsInsights.Interfaces;
using Xunit;

namespace SmsInsights.Tests.LoadTests;

public class RateLimiterLoadTests
{
    private readonly IRedisService _redisService;
    private readonly IRateLimiterService _rateLimiterService;
    private const int MaxMessagesPerSender = 10;
    private const int MaxMessagesGlobal = 50;

    public RateLimiterLoadTests()
    {
        _redisService = Substitute.For<IRedisService>();
        _rateLimiterService = new RateLimiterService(_redisService, MaxMessagesPerSender, MaxMessagesGlobal);
    }

    [Fact]
    public async Task MultipleSimultaneousRequests_RespectSenderRateLimit()
    {
        // Arrange
        const string senderNumber = "+1234567890";
        const int numberOfRequests = 100;
        var successfulRequests = new ConcurrentBag<bool>();
        var stopwatch = new Stopwatch();

        // Configure Redis mock to actually count requests
        var requestCount = 0;
        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(x => Interlocked.Increment(ref requestCount) <= MaxMessagesPerSender);

        // Act
        stopwatch.Start();
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(_ => Task.Run(() =>
            {
                var result = _rateLimiterService.CanSend(senderNumber);
                successfulRequests.Add(result);
                return result;
            }));

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        var successCount = successfulRequests.Count(x => x);
        Assert.Equal(MaxMessagesPerSender, successCount);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Load test took too long to complete");
    }

    [Fact]
    public async Task MultipleSimultaneousRequests_RespectGlobalRateLimit()
    {
        // Arrange
        const int numberOfRequests = 200;
        var successfulRequests = new ConcurrentBag<bool>();
        var stopwatch = new Stopwatch();

        // Configure Redis mock to actually count requests
        var requestCount = 0;
        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(x => Interlocked.Increment(ref requestCount) <= MaxMessagesGlobal);

        // Act
        stopwatch.Start();
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(_ => Task.Run(() =>
            {
                var result = _rateLimiterService.CanSendGlobal();
                successfulRequests.Add(result);
                return result;
            }));

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        var successCount = successfulRequests.Count(x => x);
        Assert.Equal(MaxMessagesGlobal, successCount);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Load test took too long to complete");
    }

    [Fact]
    public async Task MultipleSimultaneousRequests_FromDifferentSenders()
    {
        // Arrange
        const int sendersCount = 5;
        const int requestsPerSender = 20;
        var successfulRequests = new ConcurrentDictionary<string, int>();
        var stopwatch = new Stopwatch();

        // Track counts per sender
        var senderCounts = new ConcurrentDictionary<string, int>();

        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(x =>
            {
                var key = x.Arg<string>();
                var currentCount = senderCounts.AddOrUpdate(
                    key, 
                    1, 
                    (_, count) => count + 1);
                return currentCount <= MaxMessagesPerSender;
            });

        // Act
        stopwatch.Start();
        var tasks = Enumerable.Range(0, sendersCount)
            .SelectMany(senderIndex =>
            {
                var senderNumber = $"+1234567{senderIndex:D3}";
                return Enumerable.Range(0, requestsPerSender)
                    .Select(_ => Task.Run(() =>
                    {
                        var result = _rateLimiterService.CanSend(senderNumber);
                        if (result)
                        {
                            successfulRequests.AddOrUpdate(
                                senderNumber,
                                1,
                                (_, count) => count + 1);
                        }
                        return result;
                    }));
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        Assert.Equal(sendersCount, successfulRequests.Count);
        Assert.All(successfulRequests.Values, count => Assert.Equal(MaxMessagesPerSender, count));
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Load test took too long to complete");
    }

    [Fact]
    public async Task RealWorldScenario_MixedSenderAndGlobalLimits()
    {
        // Arrange
        const int sendersCount = 10;
        const int requestsPerSender = 15;
        var globalCount = 0;
        var senderCounts = new ConcurrentDictionary<string, int>();
        var results = new ConcurrentBag<(string Sender, bool Success)>();

        _redisService.IncrementWithExpiration(Arg.Any<string>(), Arg.Any<int>())
            .Returns(x =>
            {
                var key = x.Arg<string>();
                if (key.StartsWith("global"))
                {
                    return Interlocked.Increment(ref globalCount) <= MaxMessagesGlobal;
                }
                
                return senderCounts.AddOrUpdate(
                    key, 
                    1, 
                    (_, count) => count + 1) <= MaxMessagesPerSender;
            });

        // Act
        var tasks = Enumerable.Range(0, sendersCount)
            .SelectMany(senderIndex =>
            {
                var senderNumber = $"+1234567{senderIndex:D3}";
                return Enumerable.Range(0, requestsPerSender)
                    .Select(_ => Task.Run(() =>
                    {
                        // Check both sender and global limits
                        var canSend = _rateLimiterService.CanSend(senderNumber) && 
                                    _rateLimiterService.CanSendGlobal();
                        results.Add((senderNumber, canSend));
                    }));
            });

        await Task.WhenAll(tasks);

        // Assert
        var successfulRequests = results.Count(r => r.Success);
        Assert.True(successfulRequests <= MaxMessagesGlobal, 
            "Global rate limit was exceeded");
        
        foreach (var senderGroup in results.GroupBy(r => r.Sender))
        {
            var senderSuccesses = senderGroup.Count(r => r.Success);
            Assert.True(senderSuccesses <= MaxMessagesPerSender, 
                $"Sender {senderGroup.Key} exceeded rate limit");
        }
    }
} 