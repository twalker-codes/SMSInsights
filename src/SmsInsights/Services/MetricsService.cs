using System;
using System.Collections.Generic;
using System.Linq;
using SmsInsights.Interfaces;
using SmsInsights.Models;

namespace SmsInsights.Services
{
    public class MetricsService : IMetricsService
    {
        private readonly IRedisService _redisService;
        private readonly IRateLimiterService _rateLimiterService;

        public MetricsService(IRedisService redisService, IRateLimiterService rateLimiterService)
        {
            _redisService = redisService;
            _rateLimiterService = rateLimiterService;
        }

        public AggregatedSenderMetrics GetAggregatedSenderMetrics(string senderNumber, DateTime from, DateTime to)
        {
            var keys = GetKeysInRange($"rate_limit:{senderNumber}", from, to);
            var counts = keys.Select(k => _redisService.GetCount(k));
            var totalCount = counts.Sum();
            var maxPossible = _rateLimiterService.GetMaxMessagesPerSender() * (to - from).TotalSeconds;

            return new AggregatedSenderMetrics
            {
                SenderNumber = senderNumber,
                AverageUsagePercentage = maxPossible > 0 ? (totalCount * 100.0) / maxPossible : 0,
                TotalMessageCount = totalCount,
                FromTime = from,
                ToTime = to
            };
        }

        public AggregatedGlobalMetrics GetAggregatedGlobalMetrics(DateTime from, DateTime to)
        {
            var keys = GetKeysInRange("global_rate_limit", from, to);
            var counts = keys.Select(k => _redisService.GetCount(k));
            var totalCount = counts.Sum();
            var maxPossible = _rateLimiterService.GetMaxMessagesGlobal() * (to - from).TotalSeconds;
            
            return new AggregatedGlobalMetrics
            {
                AverageUsagePercentage = maxPossible > 0 ? (totalCount * 100.0) / maxPossible : 0,
                TotalMessageCount = totalCount,
                FromTime = from,
                ToTime = to
            };
        }

        private IEnumerable<string> GetKeysInRange(string prefix, DateTime from, DateTime to)
        {
            var current = from;
            while (current <= to)
            {
                yield return $"{prefix}:{current:yyyyMMddHHmmss}";
                current = current.AddSeconds(1);
            }
        }
    }
} 