using System;
using SmsInsights.Models;

namespace SmsInsights.Interfaces
{
    public interface IMetricsService
    {
        AggregatedSenderMetrics GetAggregatedSenderMetrics(string senderNumber, DateTime from, DateTime to);
        AggregatedGlobalMetrics GetAggregatedGlobalMetrics(DateTime from, DateTime to);
    }
} 