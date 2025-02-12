namespace SmsInsights.Models;

/// <summary>
/// Represents an SMS message request.
/// </summary>
public class SmsRequest
{
    public string SenderPhoneNumber { get; set; } = string.Empty;
    public string ReceiverPhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
