namespace SmsInsights.Models;

public class AggregatedSenderMetrics
{
    public string SenderNumber { get; set; } = string.Empty;
    public double AverageUsagePercentage { get; set; }
    public int TotalMessageCount { get; set; }
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
} 