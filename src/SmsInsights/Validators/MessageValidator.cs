using SmsInsights.Models;

namespace SmsInsights.Validators;

public static class MessageValidator
{
    public static (bool IsValid, string? ErrorMessage) ValidateRequest(SmsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SenderPhoneNumber))
            return (false, "Sender phone number is required.");
            
        if (string.IsNullOrWhiteSpace(request.ReceiverPhoneNumber))
            return (false, "Receiver phone number is required.");
            
        if (string.IsNullOrWhiteSpace(request.Message))
            return (false, "Message content is required.");

        return (true, null);
    }
} 