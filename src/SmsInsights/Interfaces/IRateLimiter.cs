namespace SmsInsights.Interfaces;

public interface IRateLimiterService
{
    bool CanSend(string senderPhoneNumber);
    bool CanSendGlobal();
}
