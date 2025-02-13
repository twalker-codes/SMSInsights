namespace SmsInsights.Models
{
    public class AggregatedGlobalMetrics
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TotalMessages { get; set; }
        public double AverageMessagesPerSecond { get; set; }
        public int MaxMessagesInOneSecond { get; set; }
        public int UsagePercentage { get; set; }
        public DateTime Timestamp { get; set; }
        public double AverageUsagePercentage { get; set; }
        public int TotalMessageCount { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
} 