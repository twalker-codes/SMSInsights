namespace SmsInsights.Models;

public class AggregatedGlobalMetrics
{
    public double AverageUsagePercentage { get; set; }
    public int TotalMessageCount { get; set; }
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
} 